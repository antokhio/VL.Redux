using VL.Core.Import;

namespace VL.Redux
{
    [Name("ISlice (Ungeneric)")]
    public interface ISlice
    {
        Type ModelType { get; }

        object InitialModel { get; }

        object Reduce(object model, IAction action);
    }

    public interface ISlice<TModel> : ISlice
      where TModel : class
    {
        TModel InitialState { get; }

        TModel Reduce(TModel state, IAction action);
    }

    public abstract class Slice<TModel> : ISlice<TModel>
       where TModel : class
    {
        public abstract TModel InitialState { get; }

        public virtual TModel Reduce(TModel state, IAction action)
            => state;

        Type ISlice.ModelType => typeof(TModel);

        object ISlice.InitialModel => InitialState;

        object ISlice.Reduce(object model, IAction action)
            => Reduce((TModel)model, action);
    }
}
