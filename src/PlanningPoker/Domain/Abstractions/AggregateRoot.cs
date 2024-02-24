#region

using PlanningPoker.Domain.Abstractions.Clock;
using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Domain.Abstractions;

public abstract class AggregateRoot : Entity, IAggregateRoot
{
    private readonly IList<IDomainEvent> _domainEvents = new List<IDomainEvent>();

    protected AggregateRoot(string id, IDateTimeProvider dateTimeProvider)
        : base(id, dateTimeProvider)
    {
    }

    protected AggregateRoot(string id)
        : base(id)
    {
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}

public abstract class TenantableAggregateRoot : AggregateRoot, ITenantable
{
    protected TenantableAggregateRoot(string id, string tenantId, IDateTimeProvider dateTimeProvider) : base(id,
        dateTimeProvider)
    {
        SetTenant(tenantId);
    }

    protected TenantableAggregateRoot(string id, string tenantId) : base(id)
    {
        SetTenant(tenantId);
    }

    public EntityId TenantId { get; private set; } = EntityId.Empty;

    public void SetTenant(string tenantId)
    {
        if (TenantId.Value.IsPresent())
        {
            AddError(new Error(nameof(TenantId), "Tenant id cannot be changed."));
            return;
        }

        if (tenantId.IsEmpty())
        {
            AddError(Error.NullOrEmpty(code: nameof(TenantId)));
            return;
        }

        TenantId = tenantId;
    }
}