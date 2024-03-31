#region

using PlanningPoker.Application.Tenants;
using PlanningPoker.Application.Users;

#endregion

namespace PlanningPoker.Application.Security;

public record SecurityInformation(TenantInformation Tenant, UserInformation User);