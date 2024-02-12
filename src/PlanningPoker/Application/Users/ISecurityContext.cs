namespace PlanningPoker.Application.Users
{
    public interface ISecurityContext
    {
        Task<SecurityInformation> GetSecurityInformationAsync();
    }
}