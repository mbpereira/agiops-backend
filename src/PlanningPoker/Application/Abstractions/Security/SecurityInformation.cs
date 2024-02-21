using PlanningPoker.Application.Tenants;
using PlanningPoker.Application.Users;

namespace PlanningPoker.Application.Abstractions.Security
{
    public record SecurityInformation(TenantInformation Tenant, UserInformation User);
}