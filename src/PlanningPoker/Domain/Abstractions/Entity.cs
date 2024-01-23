using Domain.Validation;

namespace Domain.Abstractions
{

    public abstract class Entity<TEntity> : IEntity
        where TEntity : Entity<TEntity>
    {
        private readonly Validator<TEntity> _validator;
        private readonly IList<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public EntityId Id { get; protected set; }

        public Entity(EntityId id)
        {
            Id = id;
            _validator = new Validator<TEntity>();
            ConfigureValidationRules(_validator);
        }

        protected abstract void ConfigureValidationRules(Validator<TEntity> validator);

        public virtual ValidationResult Validate() => new ValidationResult(_validator.Validate((TEntity)this));

        public IReadOnlyList<IDomainEvent> GetDomainEvents()
            => _domainEvents.ToList();

        public void ClearDomainEvents()
            => _domainEvents.Clear();

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);
    }
}
