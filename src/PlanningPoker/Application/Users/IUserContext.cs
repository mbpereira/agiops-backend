namespace PlanningPoker.Application.Users;

public interface IUserContext
{
    Task<UserInformation> GetCurrentUserAsync();
}