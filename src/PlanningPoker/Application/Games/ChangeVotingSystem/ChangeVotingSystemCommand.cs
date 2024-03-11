using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Common.Helpers;
using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Games;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Games.ChangeVotingSystem;

public class ChangeVotingSystemCommand : Command
{
    public string Id { get; private set; } = string.Empty;
    private ChangeVotingSystemData Data { get; }

    public ChangeVotingSystemCommand(string id, ChangeVotingSystemData data)
    {
        SetId(id);
        Data = data;
    }

    private void SetId(string id)
    {
        if (id.IsNullOrEmpty())
        {
            AddError(Error.NullOrEmpty(nameof(ChangeVotingSystemCommand), nameof(Id)));
            return;
        }

        Id = id;
    }

    public bool ApplyChangesTo(VotingSystem votingSystem)
    {
        var hasAnyChange = false;

        hasAnyChange |= Actions.ExecuteIfNotNull(Data.Name, votingSystem.SetName);
        hasAnyChange |= Actions.ExecuteIfNotNull(Data.Description, votingSystem.SetDescription);
        hasAnyChange |= Actions.ExecuteIfNotNull(Data.PossibleGrades, votingSystem.SetPossibleGrades);

        return hasAnyChange;
    }
}