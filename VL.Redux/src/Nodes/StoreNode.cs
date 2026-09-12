using Microsoft.Extensions.Logging;
using VL.Core;
using VL.Core.Import;
using VL.Lib.Collections;
using VL.Model;

namespace VL.Redux
{
    [ProcessNode(Name = "Store")]
    public class StoreNode
    {
        private readonly IStore<State> _store;
        private ILogger _looger;

        public StoreNode(
            [Pin(Visibility = PinVisibility.Hidden)] NodeContext nodeContext,
            [Pin(PinGroupKind = PinGroupKind.Collection, PinGroupDefaultCount = 1)]
                Spread<ISlice> slices
        )
        {
            _looger = nodeContext.GetLogger();

            if (slices is null)
                throw new ArgumentNullException(nameof(slices));

            var reducer = new Reducer(slices);

            _store = new Store<State>(reducer.InitialState, reducer.Reduce);
        }

        public IStore<State> Output => _store;
    }
}
