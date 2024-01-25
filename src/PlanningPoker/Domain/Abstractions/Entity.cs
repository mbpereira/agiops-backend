using Domain.Validation;

namespace Domain.Abstractions
{

    public abstract class Entity<TEntity>
        where TEntity : Entity<TEntity>
    {
        private readonly Validator<TEntity> _validator;

        public EntityId Id { get; private set; }

        public Entity(EntityId id)
        {
            Id = id;
            _validator = new Validator<TEntity>();
            ConfigureValidationRules(_validator);
        }

        protected abstract void ConfigureValidationRules(Validator<TEntity> validator);

        public virtual ValidationResult Validate()
            => new(_validator.Validate((TEntity)this));
    }
}
