namespace PlanningPoker.Application.Security.User
{
    public interface IUserContext
    {
        Task<UserInformation> GetCurrentUserAsync();
    }
}
