namespace PlanningPoker.Application.Security.Tenant
{
    public interface ITenantContext
    {
        Task<TenantInformation> GetCurrentTenantAsync();
    }
}
