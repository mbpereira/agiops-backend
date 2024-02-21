using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Abstractions
{
    public abstract class AggregateRoot(int id) : Entity(id), IAggregateRoot
    {
        private readonly IList<IDomainEvent> _domainEvents = new List<IDomainEvent>();

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
            SetTenant(tenantId);
        }

        public void SetTenant(int tenantId)
        {
            if (TenantId.Value.GreaterThan(0))
            {
                AddError(new Error(nameof(TenantId), "Tenant id cannot be changed."));
                return;
            }

            if (!tenantId.GreaterThan(0))
            {
                AddError(Error.GreaterThan(code: nameof(TenantId), value: 0));
                return;
            }

            TenantId = tenantId;
        }
    }
}