using System.Reactive.Linq;
using System.Reactive.Subjects;
using VL.Core.Import;

namespace VL.Redux
{
    [Name("IStore (Ungeneric)")]
    public interface IStore : IDisposable
    {
        void Dispatch(IAction action);
        void Dispatch<TModel>(Func<TModel, TModel> func)
            where TModel : class;
    }

    public interface IStore<TStore> : IStore, IObservable<TStore>
    {
        public IObservable<TStore> State { get; }
        public TStore Current { get; }
    }

    public class Store<TStore> : IStore<TStore>
    {
        private readonly object _gate = new();
        private readonly Queue<IAction> _pending = new();
        private readonly Subject<IAction> _actions = new();
        private readonly BehaviorSubject<TStore> _state;
        private readonly IDisposable _reduction;

        private bool _draining;
        private bool _disposed;
        private Exception? _fault;

        public IObservable<TStore> State { get; }

        public Store(
           TStore initialState,
           Func<TStore, IAction, TStore> reducer)
        {
            if (reducer is null)
                throw new ArgumentNullException(nameof(reducer));

            _state = new BehaviorSubject<TStore>(initialState);
            State = _state.AsObservable();

            // The store owns one reduction subscription for its lifetime.
            _reduction = _actions
                .Scan(initialState, reducer)
                .DistinctUntilChanged()
                .Subscribe(
                    next => _state.OnNext(next),
                    error =>
                    {
                        _fault = error;
                        _state.OnError(error);
                    });
        }

        public TStore Current
        {
            get
            {
                lock (_gate)
                {
                    if (_disposed)
                    {
                        throw new ObjectDisposedException(
                            nameof(Store<TStore>));
                    }

                    ThrowIfFaulted();

                    return _state.Value;
                }
            }
        }

        IDisposable IObservable<TStore>.Subscribe(
            IObserver<TStore> observer)
        {
            return State.Subscribe(observer);
        }

        private void ThrowIfFaulted()
        {
            if (_fault is not null)
            {

                throw new InvalidOperationException(
                    "The store's reducer pipeline has faulted.",
                    _fault);
            }
        }

        public void Dispatch(IAction action)
        {
            lock (_gate)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(
                        nameof(Store<TStore>));
                }

                ThrowIfFaulted();

                _pending.Enqueue(action);

                // Reentrant dispatch queues the action instead of
                // interrupting the current state notification.
                if (_draining)
                    return;

                _draining = true;

                try
                {
                    while (!_disposed && _pending.Count > 0)
                    {
                        _actions.OnNext(_pending.Dequeue());
                        ThrowIfFaulted();
                    }
                }
                finally
                {
                    _pending.Clear();
                    _draining = false;
                }
            }
        }

        public void Dispatch<TModel>(Func<TModel, TModel> func)
            where TModel : class
        {
            Dispatch(new SliceAction<TModel>(func));
        }

        public void Dispose()
        {
            lock (_gate)
            {
                if (_disposed)
                    return;

                _disposed = true;
                _pending.Clear();

                _reduction.Dispose();
                _actions.Dispose();
                _state.Dispose();
            }
        }
    }
}
