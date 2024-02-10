using PlanningPoker.Application.Security.Tenant;
using PlanningPoker.Application.Security.User;

namespace PlanningPoker.Application.Security
{
    public record SecurityInformation(TenantInformation Tenant, UserInformation User);
}