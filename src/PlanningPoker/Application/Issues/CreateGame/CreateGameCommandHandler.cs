using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;

namespace PlanningPoker.Application.Issues.CreateGame
{
    public class CreateGameCommandHandler : ICommandHandler<CreateGameCommand, CreateGameResult>
    {
        private readonly IUnitOfWork _uow;
        private readonly ISecurityContext _security;

        public CreateGameCommandHandler(IUnitOfWork uow, ISecurityContext authenticationContext)
        {
            _uow = uow;
            _security = authenticationContext;
        }

        public async Task<CommandResult<CreateGameResult>> HandleAsync(CreateGameCommand command)
        {
            if (!command.IsValid)
                return CommandResult<CreateGameResult>.Fail(command.Errors, CommandStatus.ValidationFailed);

            var votingSystem = await _uow.VotingSystems.GetByIdAsync(command.VotingSystemId);

            if (votingSystem is null)
                return CommandResult<CreateGameResult>.Fail(CommandStatus.RecordNotFound);

            var context = await _security.GetSecurityInformationAsync();

            var game = Game.New(context.Tenant.Id, command.Name, context.User.Id, votingSystem, command.Password);

            if (!game.IsValid)
                return CommandResult<CreateGameResult>.Fail(game.Errors, CommandStatus.ValidationFailed);

            var createdGame = await _uow.Games.AddAsync(game);

            await _uow.SaveChangesAsync();

            return CommandResult<CreateGameResult>.Success(new CreateGameResult(createdGame.Id.Value));
        }
    }
}