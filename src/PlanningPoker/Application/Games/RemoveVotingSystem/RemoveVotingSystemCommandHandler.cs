using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Application.Games.RemoveVotingSystem;

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