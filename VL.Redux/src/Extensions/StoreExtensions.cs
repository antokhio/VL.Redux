using System.Reactive.Linq;
using VL.Core.Import;

namespace VL.Redux
{
    public static class StoreExtensions
    {
        [Name("Select (Stateless)")]
        public static TValue Select<TModel, TValue>(
            this IStore<TModel> store,
            Func<TModel, TValue> selector
        )
        {
            if (store is null)
                throw new ArgumentNullException(nameof(store));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            // Capture one snapshot, then run user code outside
            // the store's snapshot-acquisition lock.
            var snapshot = store.Current;

            return selector(snapshot);
        }

        [Name("Select (Stateless Observable)")]
        public static IObservable<TValue> Select<TModel, TValue>(
            this IStore<TModel> store,
            Func<TModel, TValue> selector,
            IEqualityComparer<TValue>? comparer = null
        )
        {
            if (store is null)
                throw new ArgumentNullException(nameof(store));
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return store
                .State.Select(selector)
                .DistinctUntilChanged(comparer ?? EqualityComparer<TValue>.Default);
        }

        [Name("OfType (Stateless Observable)")]
        public static IObservable<TModel> OfType<TModel>(
            this IStore<State> store,
            IEqualityComparer<TModel>? comparer = null
        )
            where TModel : class => store.Select(state => state.OfType<TModel>(), comparer);
    }
}
