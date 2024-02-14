using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Abstractions
{
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        private readonly IList<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        protected AggregateRoot(int id) : base(id)
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
            DefineTenant(tenantId);
        }

        public void DefineTenant(int tenantId)
        {
            if (TenantId.Value.GreaterThan(0)) return;

            if (!tenantId.GreaterThan(0))
            {
                AddError(Error.GreaterThan(code: nameof(tenantId), value: 0));
                return;
            }

            TenantId = tenantId;
        }
    }
}
