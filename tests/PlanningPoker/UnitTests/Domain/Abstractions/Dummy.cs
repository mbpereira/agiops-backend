#region

using PlanningPoker.Domain.Abstractions;

#endregion

namespace PlanningPoker.UnitTests.Domain.Abstractions;

public class Dummy(string id, string tenantId) : TenantableAggregateRoot(id, tenantId);