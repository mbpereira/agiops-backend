#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Games;
using PlanningPoker.Domain.Validation;
using static PlanningPoker.Application.Common.Helpers.Actions;

#endregion

namespace PlanningPoker.Application.Games.VotingSystems.ChangeVotingSystem;

public class ChangeVotingSystemCommand : Command
{
    public ChangeVotingSystemCommand(string id, ChangeVotingSystemCommandPayload payload)
    {
        SetId(id);
        Payload = payload;
    }

    public string Id { get; private set; } = string.Empty;
    private ChangeVotingSystemCommandPayload Payload { get; }

    private void SetId(string id)
    {
        if (!id.IsPresent())
        {
            AddError(Error.NullOrEmpty(nameof(ChangeVotingSystemCommand), nameof(Id)));
            return;
        }

        Id = id;
    }

    public bool ApplyChangesTo(VotingSystem votingSystem)
    {
        var hasAnyChange = false;

        hasAnyChange |= ExecuteIfNotNull(Payload.Name, votingSystem.SetName);
        hasAnyChange |= ExecuteIfNotNull(Payload.PossibleGrades, votingSystem.SetPossibleGrades);
        hasAnyChange |= ExecuteIfNotNull(Payload.Description, votingSystem.SetDescription);

        if (hasAnyChange) votingSystem.Updated();

        return hasAnyChange;
    }
}