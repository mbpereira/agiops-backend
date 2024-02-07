namespace PlanningPoker.Application.Security
{
    public interface IAuthenticationContext
    {
        Task<UserInformation?> GetCurrentUserAsync();
    }
}