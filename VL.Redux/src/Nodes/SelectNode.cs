using System.Reactive.Linq;
using System.Reactive.Subjects;
using VL.Core.Import;

namespace VL.Redux.Nodes
{
    [ProcessNode(Name = "Select (Reactive)")]
    public sealed class SelectorNode<TValue> :
        IObservable<TValue>,
        IDisposable
    {
        private readonly Subject<IStore<State>>
            _storeInputs = new();

        private readonly Subject<Func<State, TValue>>
            _selectorInputs = new();

        private readonly ReplaySubject<TValue> _result = new(1);
        private readonly IDisposable _subscription;

        private IStore<State>? _store;
        private Func<State, TValue>? _selector;
        private bool _disposed;

        public SelectorNode()
        {
            // Keep the output observable stable across frames.
            Result = _result.AsObservable();

            _subscription = _storeInputs
                // Observe the current store's state stream.
                .Select(store => store.State)
                .Switch()

                // Re-evaluate when state or selector changes.
                .CombineLatest(
                    _selectorInputs,
                    (state, selector) => selector(state))

                .DistinctUntilChanged()
                .Subscribe(_result);
        }

        public void SetStore(IStore<State> store)
        {
            ThrowIfDisposed();

            if (store is null)
                throw new ArgumentNullException(nameof(store));

            // VL can call this every frame without resubscribing.
            if (ReferenceEquals(_store, store))
                return;

            _store = store;
            _storeInputs.OnNext(store);
        }

        public void SetSelector(Func<State, TValue> selector)
        {
            ThrowIfDisposed();

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            if (Equals(_selector, selector))
                return;

            _selector = selector;
            _selectorInputs.OnNext(selector);
        }

        public IObservable<TValue> Result { get; }

        IDisposable IObservable<TValue>.Subscribe(
            IObserver<TValue> observer)
        {
            ThrowIfDisposed();
            return _result.Subscribe(observer);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _subscription.Dispose();
            _storeInputs.Dispose();
            _selectorInputs.Dispose();
            _result.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    nameof(SelectorNode<TValue>));
            }
        }
    }
}
