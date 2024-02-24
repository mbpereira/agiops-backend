#region

using PlanningPoker.Domain.Abstractions;

#endregion

namespace PlanningPoker.UnitTests.Domain.Abstractions;

public class Dummy : TenantableAggregateRoot
{
    public Dummy(string id, string tenantId) : base(id, tenantId)
    {
    }
}