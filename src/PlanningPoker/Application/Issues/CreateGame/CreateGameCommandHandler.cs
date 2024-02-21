﻿using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;

namespace PlanningPoker.Application.Issues.CreateGame
{
    public class CreateGameCommandHandler(IUnitOfWork uow, ISecurityContext authenticationContext)
        : ICommandHandler<CreateGameCommand, CreateGameResult>
    {
        public async Task<CommandResult<CreateGameResult>> HandleAsync(CreateGameCommand command)
        {
            if (!command.IsValid)
                return CommandResult<CreateGameResult>.Fail(command.Errors, CommandStatus.ValidationFailed);

            var votingSystem = await uow.VotingSystems.GetByIdAsync(command.VotingSystemId);

            if (votingSystem is null)
                return CommandResult<CreateGameResult>.Fail(CommandStatus.RecordNotFound);

            var context = await authenticationContext.GetSecurityInformationAsync();

            var game = Game.New(context.Tenant.Id, command.Name, context.User.Id, votingSystem, command.Password);

            if (!game.IsValid)
                return CommandResult<CreateGameResult>.Fail(game.Errors, CommandStatus.ValidationFailed);

            var createdGame = await uow.Games.AddAsync(game);

            await uow.SaveChangesAsync();

            return CommandResult<CreateGameResult>.Success(new CreateGameResult(createdGame.Id.Value));
        }
    }
}