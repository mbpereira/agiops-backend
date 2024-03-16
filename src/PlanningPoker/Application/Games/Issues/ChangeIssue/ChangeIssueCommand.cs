#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Games;
using PlanningPoker.Domain.Validation;
using static PlanningPoker.Application.Common.Helpers.Actions;

#endregion

namespace PlanningPoker.Application.Games.Issues.ChangeIssue;

public class ChangeIssueCommand : Command
{
    public ChangeIssueCommand(string id, ChangeIssueCommandPayload payload)
    {
        Payload = payload;
        SetId(id);
    }

    public string Id { get; private set; } = string.Empty;
    private ChangeIssueCommandPayload Payload { get; }

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

        hasAnyChange |= ExecuteIfNotNull(Payload.Name, issue.SetName);
        hasAnyChange |= ExecuteIfNotNull(Payload.Description, issue.SetDescription);
        hasAnyChange |= ExecuteIfNotNull(Payload.Link, issue.SetLink);

        if (hasAnyChange) issue.Updated();

        return hasAnyChange;
    }
}