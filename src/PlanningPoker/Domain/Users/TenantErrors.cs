using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users;

public static class TenantErrors
{
    public static readonly Error InvalidName = Error.MinLength(nameof(Tenant), nameof(Tenant.Name), minLength: 3);
}