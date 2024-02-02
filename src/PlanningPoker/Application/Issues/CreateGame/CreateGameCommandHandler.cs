using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;

namespace PlanningPoker.Application.Issues.CreateGame
{
    public class CreateGameCommandHandler
    {
        private readonly IUnitOfWork _uow;

        public CreateGameCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CommandResult<CreateGameResult>> HandleAsync(CreateGameCommand command)
        {
            var game = Game.New(command.Name, command.UserId, command.Password);

            var validationResult = game.Validate();

            if (!validationResult.Success)
                return CommandResult<CreateGameResult>.Fail(validationResult.Errors, CommandStatus.ValidationFailed);

            var createdGame = await _uow.Games.AddAsync(game);

            await _uow.SaveChangesAsync();

            return CommandResult<CreateGameResult>.Success(new CreateGameResult(createdGame.Id.Value));
        }
    }
}