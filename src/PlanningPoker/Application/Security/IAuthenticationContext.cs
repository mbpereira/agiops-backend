
namespace PlanningPoker.UnitTests.Application.Issues.RegisterGrade
{
    public interface IAuthenticationContext
    {
        Task<UserInformation?> GetCurrentUserAsync();
    }
}