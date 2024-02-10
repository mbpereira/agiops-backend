namespace PlanningPoker.Application.Security.Authentication
{
    public interface IAuthenticationContext
    {
        Task<UserInformation> GetCurrentUserAsync();
    }
}
