using System.Collections.Immutable;

namespace VL.Redux
{
    public sealed class State
    {
        private readonly ImmutableDictionary<Type, object> _models;

        internal static State Empty { get; } = new(
            ImmutableDictionary<Type, object>.Empty);

        private State(ImmutableDictionary<Type, object> models)
        {
            _models = models;
        }

        public TModel OfType<TModel>() where TModel : class
            => (TModel)Get(typeof(TModel));

        internal object Get(Type modelType)
        {
            if (!_models.TryGetValue(modelType, out var model))
            {
                throw new KeyNotFoundException(
                    $"No slice registered for {modelType.Name}.");
            }

            return model;
        }

        internal State With(Type modelType, object model)
        {
            if (model is null || !modelType.IsInstanceOfType(model))
            {
                throw new ArgumentException(
                    $"Expected a non-null {modelType.Name}.",
                    nameof(model));
            }

            if (_models.TryGetValue(modelType, out var previous) &&
                ReferenceEquals(previous, model))
            {
                return this;
            }

            var models = _models.SetItem(modelType, model);

            return ReferenceEquals(models, _models)
                ? this
                : new State(models);
        }
    }
}
