#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Abstractions.Security;
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
            return CommandResult<AddGameResult>.Fail(command.Errors, CommandStatus.ValidationFailed);

        var votingSystem = await uow.VotingSystems.GetByIdAsync(command.VotingSystemId);

        if (votingSystem is null)
            return CommandResult<AddGameResult>.Fail(CommandStatus.RecordNotFound);

        var context = await authenticationContext.GetSecurityInformationAsync();

        var game = Game.New(context.Tenant.Id, command.Name, context.User.Id, votingSystem, command.Password,
            command.TeamId);

        if (!game.IsValid)
            return CommandResult<AddGameResult>.Fail(game.Errors, CommandStatus.ValidationFailed);

        var createdGame = await uow.Games.AddAsync(game);

        await uow.SaveChangesAsync();

        return CommandResult<AddGameResult>.Success(new AddGameResult(createdGame.Id.Value));
    }
}