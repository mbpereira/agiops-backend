namespace PlanningPoker.Domain.Abstractions
{
    public abstract class AggregateRoot<T> : Entity<T>, IAggregateRoot where T : AggregateRoot<T>
    {
        private readonly IList<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public AggregateRoot(EntityId id) : base(id)
        {
        }

        public IReadOnlyList<IDomainEvent> GetDomainEvents()
            => _domainEvents.ToList();

        public void ClearDomainEvents()
            => _domainEvents.Clear();

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);
    }
}
