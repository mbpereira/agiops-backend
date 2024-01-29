namespace PlanningPoker.UnitTests.Application.CreateGame
{
    public record CreateGameCommand(string Name, int UserId, string? Password = null)
    {
        public Task<CommandResult> HandleAsync(CreateGameCommand command)
        {
            throw new NotImplementedException();
        }
    }
}