using VL.Core.Import;

namespace VL.Redux
{

    [ProcessNode(Name = "Slice", HasStateOutput = true, FragmentSelection = FragmentSelection.Explicit)]
    public class SliceNode<TModel> : Slice<TModel>, ISlice
        where TModel : class
    {
        [Fragment]
        public SliceNode(TModel initialState)
        {
            InitialState = initialState
                ?? throw new ArgumentNullException(nameof(initialState));
        }

        public override TModel InitialState { get; }
    }
}
