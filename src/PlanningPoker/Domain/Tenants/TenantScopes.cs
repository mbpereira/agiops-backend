#region

using PlanningPoker.Domain.Users;

#endregion

namespace PlanningPoker.Domain.Tenants;

public static class TenantScopes
{
    private static GrantScopes[] Admin =>
        [GrantScopes.Delete, GrantScopes.View, GrantScopes.Archive, GrantScopes.Edit];

    private static GrantScopes[] Viewer => [GrantScopes.View];

    public static GrantScopes[] GetByRole(Role role)
    {
        return Role.Admin.Equals(role) ? Admin : Viewer;
    }
}