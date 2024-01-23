using Domain.Validation;

namespace Domain.Abstractions
{

    public abstract class Entity<TEntity>
        where TEntity : Entity<TEntity>
    {
        private readonly Validator<TEntity> _validator;

        public EntityId Id { get; protected set; }

        public Entity(EntityId id)
        {
            Id = id;
            _validator = new Validator<TEntity>();
        }

        protected abstract void ConfigureValidationRules(Validator<TEntity> validator);

        public virtual ValidationResult Validate()
        {
            ConfigureValidationRules(_validator);
            return new ValidationResult(_validator.Validate((TEntity)this));
        }
    }
}
