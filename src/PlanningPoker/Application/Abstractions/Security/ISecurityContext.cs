namespace PlanningPoker.Application.Abstractions.Security;

public interface ISecurityContext
{
    Task<SecurityInformation> GetSecurityInformationAsync();
}