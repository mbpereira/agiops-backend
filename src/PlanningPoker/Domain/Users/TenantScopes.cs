namespace PlanningPoker.Domain.Users
{
    public static class TenantScopes
    {
        private static GrantScopes[] Admin => new[] { GrantScopes.Delete, GrantScopes.View, GrantScopes.Archive, GrantScopes.Edit };
        private static GrantScopes[] Viewer => new[] { GrantScopes.View };

        public static GrantScopes[] GetByRole(Role role)
        {
            if (Role.Admin.Equals(role)) return Admin;
            return Viewer;
        }
    }
}
