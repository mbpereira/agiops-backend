namespace PlanningPoker.UnitTests.Application.Issues.CreateIssue
{
    public record CreateIssueCommand(int GameId, string Name, string? Link = null, string? Description = null);
}