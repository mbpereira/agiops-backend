namespace PlanningPoker.Application.Games.Issues.ChangeIssue;

public record ChangeIssueCommandPayload(string? Name = null, string? Description = null, string? Link = null);