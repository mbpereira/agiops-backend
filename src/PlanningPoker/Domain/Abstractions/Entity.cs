using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Abstractions
{

    public abstract class Entity<TEntity>
        where TEntity : Entity<TEntity>
    {
        private readonly IValidationHandler<TEntity> _validator;

        public EntityId Id { get; private set; }

        public Entity(EntityId id)
        {
            Id = id;
            _validator = new Validator<TEntity>();
            ConfigureValidationRules(_validator);
        }

        protected abstract void ConfigureValidationRules(IValidationHandler<TEntity> validator);

        public virtual ValidationResult Validate()
            => _validator.Handle((TEntity)this);
    }
}
