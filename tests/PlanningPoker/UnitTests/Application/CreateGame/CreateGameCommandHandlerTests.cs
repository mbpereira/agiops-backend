using Bogus;

namespace PlanningPoker.UnitTests.Application.CreateGame
{
    public class CreateGameCommandHandlerTests
    {
        private readonly Faker _faker;

        public CreateGameCommandHandlerTests()
        {
            _faker = new();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData(null, "abcde")]
        public async Task ShouldReturnSuccessAsFalseWhenProvidedDataIsNotValid(string invalidName, string invalidPassword)
        {
            var command = new CreateGameCommand(Name: invalidName, UserId: 0, Password: invalidPassword);
            var commandHandler = new CreateGameCommandHandler();

            var commandResult = await command.HandleAsync(command);
        }
    }
}
