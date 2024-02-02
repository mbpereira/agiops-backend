using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Issues.CreateGame;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;

namespace PlanningPoker.UnitTests.Application.Issues.CreateGame
{
    public class CreateGameCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly IUnitOfWork _uow;
        private readonly CreateGameCommandHandler _handler;

        public CreateGameCommandHandlerTests()
        {
            _uow = Substitute.For<IUnitOfWork>();
            _faker = new();
            _handler = new(_uow);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData(null, "abcde")]
        public async Task ShouldReturnValidationFailedWhenProvidedDataIsNotValid(string invalidName, string invalidPassword)
        {
            var command = new CreateGameCommand(Name: invalidName, UserId: 0, Password: invalidPassword);

            var commandResult = await _handler.HandleAsync(command);

            commandResult.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldReturnGeneratedIdWhenGameWasCreated()
        {
            var expectedGame = GetValidGame();
            var command = new CreateGameCommand(
                Name: expectedGame.Name,
                UserId: expectedGame.UserId.Value,
                Password: expectedGame.Credentials!.Password);
            _uow.Games.AddAsync(Arg.Any<Game>()).Returns(expectedGame);

            var commandResult = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            commandResult.Status.Should().Be(CommandStatus.Success);
            commandResult.Data!.Id.Should().Be(expectedGame.Id.Value);
        }

        private Game GetValidGame() => Game.New(
            id: _faker.Random.Int(min: 1, max: 100),
            name: _faker.Random.String2(length: 10),
            userId: _faker.Random.Int(min: 1, max: 100),
            password: _faker.Random.String2(length: 10));
    }
}
