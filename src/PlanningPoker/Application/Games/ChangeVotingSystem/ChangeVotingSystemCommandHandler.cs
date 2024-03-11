using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Application.Games.ChangeVotingSystem;

public class ChangeVotingSystemCommandHandler(IUnitOfWork uow)
    : ICommandHandler<ChangeVotingSystemCommand, ChangeVotingSystemResult>
{
    public async Task<CommandResult<ChangeVotingSystemResult>> HandleAsync(ChangeVotingSystemCommand command)
    {
        if (!command.IsValid)
            return (command.Errors, CommandStatus.ValidationFailed);

        var votingSystem = await uow.VotingSystems.GetByIdAsync(command.Id);

        if (votingSystem is null)
            return CommandStatus.RecordNotFound;

        var hasAnyChange = command.ApplyChangesTo(votingSystem);

        if (!hasAnyChange)
            return new ChangeVotingSystemResult(votingSystem);

        if (!votingSystem.IsValid)
            return (command.Errors, CommandStatus.ValidationFailed);

        votingSystem.Updated();

        await uow.VotingSystems.ChangeAsync(votingSystem);

        return new ChangeVotingSystemResult(votingSystem);
    }
}