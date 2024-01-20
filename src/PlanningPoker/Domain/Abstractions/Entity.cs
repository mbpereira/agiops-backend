using Domain.Validation;

namespace Domain.Abstractions
{

    public abstract class Entity<TEntity> : IEntity
        where TEntity : Entity<TEntity>
    {
        private readonly Validator<TEntity> Validator;
        private readonly IList<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public int Id { get; protected set; }

        public Entity()
        {
            Validator = new Validator<TEntity>();
            ConfigureValidationRules(Validator);
        }

        protected abstract void ConfigureValidationRules(Validator<TEntity> validator);

        public virtual ValidationResult Validate() => new ValidationResult(Validator.Validate((TEntity)this));

        public IReadOnlyList<IDomainEvent> GetDomainEvents()
            => _domainEvents.ToList();

        public void ClearDomainEvents()
            => _domainEvents.Clear();

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);
    }
}
