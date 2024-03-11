using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Games.RemoveVotingSystem;

public class RemoveVotingSystemCommand : Command
{
    public string Id { get; private set; } = string.Empty;

    public RemoveVotingSystemCommand(string id)
    {
        SetId(id);
    }

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