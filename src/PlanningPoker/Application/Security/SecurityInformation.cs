using PlanningPoker.Application.Security.Authentication;
using PlanningPoker.Application.Security.Tenant;

namespace PlanningPoker.Application.Security
{
    public record SecurityInformation(TenantInformation Tenant, UserInformation User);
}