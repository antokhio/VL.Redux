using System.Reactive.Linq;
using System.Reactive.Subjects;
using VL.Core.Import;

namespace VL.Redux.Nodes
{
    [ProcessNode(Name = "OfType (Reactive)")]
    public sealed class OfTypeNode<TModel> : IObservable<TModel>, IDisposable
        where TModel : class
    {
        private readonly Subject<IStore<State>> _storeInputs = new();
        private readonly ReplaySubject<TModel> _result = new(1);
        private readonly IDisposable _subscription;

        private IStore<State>? _store;
        private bool _disposed;

        public OfTypeNode()
        {
            // Keep the output observable stable across frames.
            Result = _result.AsObservable();

            _subscription = _storeInputs
                .Select(store => store.OfType<TModel>())
                .Switch()
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

        public IObservable<TModel> Result { get; }

        IDisposable IObservable<TModel>.Subscribe(IObserver<TModel> observer)
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
            _result.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(OfTypeNode<TModel>));
            }
        }
    }
}
