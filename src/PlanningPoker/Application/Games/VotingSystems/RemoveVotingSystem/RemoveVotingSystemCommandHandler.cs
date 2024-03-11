#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Abstractions;

#endregion

namespace PlanningPoker.Application.Games.VotingSystems.RemoveVotingSystem;

public class RemoveVotingSystemCommandHandler(IUnitOfWork uow) : ICommandHandler<RemoveVotingSystemCommand>
{
    public async Task<CommandResult> HandleAsync(RemoveVotingSystemCommand command)
    {
        if (!command.IsValid) return (command.Errors, CommandStatus.ValidationFailed);

        var votingSystem = await uow.VotingSystems.GetByIdAsync(command.Id);

        if (votingSystem is null) return CommandStatus.RecordNotFound;

        await uow.VotingSystems.RemoveAsync(votingSystem);

        return CommandStatus.Success;
    }
}