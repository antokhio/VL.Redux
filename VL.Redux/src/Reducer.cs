using VL.Lib.Collections;

namespace VL.Redux
{
    public sealed class Reducer
    {
        private readonly IReadOnlyList<ISlice> _slices;
        public State InitialState { get; }

        public Reducer(IReadOnlyList<ISlice> slices)
        {
            if (slices is null)
                throw new ArgumentNullException(nameof(slices));

            // Capture registrations once for this reducer's lifetime.
            _slices = slices.ToSpread();

            var registeredTypes = new HashSet<Type>();
            var initialState = State.Empty;

            foreach (var slice in _slices)
            {
                if (slice is null)
                {
                    throw new ArgumentException(
                        "Slice registrations cannot contain null.",
                        nameof(slices)
                    );
                }

                if (!registeredTypes.Add(slice.ModelType))
                {
                    throw new ArgumentException(
                        $"A slice for {slice.ModelType.Name} is already registered.",
                        nameof(slices)
                    );
                }

                initialState = initialState.With(slice.ModelType, slice.InitialModel);
            }

            InitialState = initialState;
        }

        public State Reduce(State state, IAction action)
        {
            if (state is null)
                throw new ArgumentNullException(nameof(state));

            if (action is null)
                throw new ArgumentNullException(nameof(action));

            // Targeted updates are handled centrally.
            if (action is ISliceAction sliceAction)
                return sliceAction.Apply(state);

            // Regular domain actions still pass through all slices.
            var nextState = state;

            foreach (var slice in _slices)
            {
                var previousModel = state.Get(slice.ModelType);
                var nextModel = slice.Reduce(previousModel, action);

                nextState = nextState.With(slice.ModelType, nextModel);
            }

            return nextState;
        }
    }
}
