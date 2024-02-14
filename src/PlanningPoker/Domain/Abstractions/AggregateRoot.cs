using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Abstractions
{
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        private readonly IList<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        protected AggregateRoot(EntityId id) : base(id)
        {
        }

        public IReadOnlyList<IDomainEvent> GetDomainEvents()
            => _domainEvents.ToList();

        public void ClearDomainEvents()
            => _domainEvents.Clear();

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);
    }

    public abstract class TenantableAggregateRoot : AggregateRoot, ITenantable
    {
        public EntityId TenantId { get; private set; } = EntityId.Blank();

        public TenantableAggregateRoot(int id, int tenantId) : base(id)
        {
            InTenant(tenantId);
        }

        public void InTenant(int tenantId)
        {
            if (!tenantId.GreaterThan(0))
            {
                AddError(Error.GreaterThan(nameof(tenantId), value: 0));
                return;
            }

            TenantId = tenantId;
        }
    }
}
