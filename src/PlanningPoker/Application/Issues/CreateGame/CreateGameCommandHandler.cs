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
            var context = await _security.GetSecurityInformationAsync();

            var game = Game.New(context.Tenant.Id, command.Name, context.User.Id, command.Password);

            var validationResult = game.Validate();

            if (!validationResult.Success)
                return CommandResult<CreateGameResult>.Fail(validationResult.Errors, CommandStatus.ValidationFailed);

            var createdGame = await _uow.Games.AddAsync(game);

            await _uow.SaveChangesAsync();

            return CommandResult<CreateGameResult>.Success(new CreateGameResult(createdGame.Id.Value));
        }
    }
}