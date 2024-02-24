#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Domain.Tenants;

public static class TenantErrors
{
    public static readonly Error InvalidName = Error.MinLength(nameof(Tenant), nameof(Tenant.Name), 3);
}