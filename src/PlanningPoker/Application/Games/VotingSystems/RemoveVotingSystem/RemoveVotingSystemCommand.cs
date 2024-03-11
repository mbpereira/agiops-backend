#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Application.Games.VotingSystems.RemoveVotingSystem;

public class RemoveVotingSystemCommand : Command
{
    public RemoveVotingSystemCommand(string id)
    {
        SetId(id);
    }

    public string Id { get; private set; } = string.Empty;

    private void SetId(string id)
    {
        if (!id.IsPresent())
        {
            AddError(Error.NullOrEmpty(nameof(RemoveVotingSystemCommand), nameof(Id)));
            return;
        }

        Id = id;
    }
}