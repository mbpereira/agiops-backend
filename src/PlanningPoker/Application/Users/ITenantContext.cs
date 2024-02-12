namespace PlanningPoker.Application.Users
{
    public interface ITenantContext
    {
        Task<TenantInformation> GetCurrentTenantAsync();
    }
}
