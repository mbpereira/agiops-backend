namespace PlanningPoker.Application.Security
{
    public interface ISecurityContext
    {
        Task<SecurityInformation> GetSecurityInformationAsync();
    }
}