using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Common.Helpers;
using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Games;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Games.Issues.ChangeIssue;

public class ChangeIssueCommand : Command
{
    public ChangeIssueCommandPayload Payload { get; }
    public string Id { get; private set; } = string.Empty;

    public ChangeIssueCommand(string id, ChangeIssueCommandPayload payload)
    {
        Payload = payload;
        SetId(id);
    }

    private void SetId(string id)
    {
        if (!id.IsPresent())
        {
            AddError(Error.NullOrEmpty(nameof(ChangeIssueCommand), nameof(Id)));
            return;
        }

        Id = id;
    }

    public bool ApplyChangesTo(Issue issue)
    {
        var hasAnyChange = false;

        hasAnyChange |= Actions.ExecuteIfNotNull(Payload?.Name, issue.SetName);
        hasAnyChange |= Actions.ExecuteIfNotNull(Payload?.Description, issue.SetDescription);
        hasAnyChange |= Actions.ExecuteIfNotNull(Payload?.Name, issue.SetLink);

        return hasAnyChange;
    }
}

public record ChangeIssueCommandPayload(string? Name = null, string? Description = null, string? Link = null);