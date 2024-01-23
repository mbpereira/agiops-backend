using Domain.Validation;

namespace Domain.Abstractions
{

    public abstract class Entity<TEntity> : IEntity
        where TEntity : Entity<TEntity>
    {
        private readonly Validator<TEntity> validator;
        private readonly IList<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public int Id { get; protected set; }

        public Entity()
        {
            validator = new Validator<TEntity>();
            ConfigureValidationRules(validator);
        }

        protected abstract void ConfigureValidationRules(Validator<TEntity> validator);

        public virtual ValidationResult Validate() => new ValidationResult(validator.Validate((TEntity)this));

        public IReadOnlyList<IDomainEvent> GetDomainEvents()
            => _domainEvents.ToList();

        public void ClearDomainEvents()
            => _domainEvents.Clear();

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);
    }
}
