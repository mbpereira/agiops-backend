using PlanningPoker.Domain.Games;

namespace PlanningPoker.Application.Games.Issues.ChangeIssue;

public record ChangeIssueResult(string Id, string GameId, string Name, string? Description, string? Link, DateTime? UpdatedAtUtc)
{
    public ChangeIssueResult(Issue issue)
        : this(issue.Id, issue.GameId, issue.Name, issue.Description, issue.Link, issue.UpdatedAtUtc)
    {
    }
}