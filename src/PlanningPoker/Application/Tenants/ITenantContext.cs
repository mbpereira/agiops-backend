namespace PlanningPoker.Application.Tenants;

public interface ITenantContext
{
    Task<TenantInformation> GetCurrentTenantAsync();
}