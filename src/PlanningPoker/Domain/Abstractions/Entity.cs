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
            
            var validator = new Validator<TEntity>();
            _validatorHandler = validator;
            
            ConfigureValidationRules(validator);
        }

        protected abstract void ConfigureValidationRules(IValidationRuleFactory<TEntity> validator);

        public virtual ValidationResult Validate()
            => _validatorHandler.Handle((TEntity)this);
    }
}
