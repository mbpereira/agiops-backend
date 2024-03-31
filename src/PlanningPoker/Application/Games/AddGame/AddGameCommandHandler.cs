#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Security;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.Application.Games.AddGame;

public class AddGameCommandHandler(IUnitOfWork uow, ISecurityContext authenticationContext)
    : ICommandHandler<AddGameCommand, AddGameResult>
{
    public async Task<CommandResult<AddGameResult>> HandleAsync(AddGameCommand command)
    {
        if (!command.IsValid)
            return (command.Errors, CommandStatus.ValidationFailed);

        var votingSystem = await uow.VotingSystems.GetByIdAsync(command.VotingSystemId);

        if (votingSystem is null)
            return CommandStatus.RecordNotFound;

        var context = await authenticationContext.GetSecurityInformationAsync();

        var game = Game.New(context.Tenant.Id, command.Name, context.User.Id, votingSystem, command.Password,
            command.TeamId);

        if (!game.IsValid)
            return (game.Errors, CommandStatus.ValidationFailed);

        var createdGame = await uow.Games.AddAsync(game);

        await uow.SaveChangesAsync();

        return new AddGameResult(createdGame.Id.Value);
    }
}