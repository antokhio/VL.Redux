namespace VL.Redux
{
    public interface IAction
    {

    }

    public interface ISliceAction : IAction
    {
        State Apply(State state);
    }

    public sealed class SliceAction<TModel> : ISliceAction
        where TModel : class
    {
        private readonly Func<TModel, TModel> _action;

        public SliceAction(Func<TModel, TModel> action)
        {
            _action = action
                ?? throw new ArgumentNullException(nameof(action));
        }

        public State Apply(State state)
        {
            if (state is null)
                throw new ArgumentNullException(nameof(state));

            // Read the latest model when the action is processed.
            var previousModel = state.OfType<TModel>();
            var nextModel = _action(previousModel);

            if (nextModel is null)
            {
                throw new InvalidOperationException(
                    $"An update for {typeof(TModel).Name} returned null.");
            }

            return state.With(typeof(TModel), nextModel);
        }
    }

}
