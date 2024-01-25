using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Abstractions
{

    public abstract class Entity<TEntity>
        where TEntity : Entity<TEntity>
    {
        private readonly IValidationHandler<TEntity> _validatorHandler;

        public EntityId Id { get; private set; }

        public Entity(EntityId id)
        {
            Id = id;
            _validatorHandler = GetValidator();
        }

        private Validator<TEntity> GetValidator()
        {
            var validator = new Validator<TEntity>();
            ConfigureValidationRules(validator);
            return validator;
        }

        protected abstract void ConfigureValidationRules(IValidationRuleFactory<TEntity> validator);

        public virtual ValidationResult Validate()
            => _validatorHandler.Handle((TEntity)this);
    }
}
