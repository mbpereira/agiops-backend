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
            var command = new CreateGameCommand(name: invalidName, userId: 0, password: invalidPassword);

            var commandResult = await _handler.HandleAsync(command);

            commandResult.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldReturnGeneratedIdWhenGameWasCreated()
        {
            var expectedGame = GetValidGame();
            var command = new CreateGameCommand(
                name: expectedGame.Name,
                userId: expectedGame.UserId.Value,
                password: expectedGame.Credentials!.Password);
            _uow.Games.AddAsync(Arg.Any<Game>()).Returns(expectedGame);

            var commandResult = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            commandResult.Status.Should().Be(CommandStatus.Success);
            commandResult.Data!.Id.Should().Be(expectedGame.Id.Value);
        }

        private Game GetValidGame() => Game.New(
            tenantId: _faker.Random.Int(min: 1),
            id: _faker.Random.Int(min: 1, max: 100),
            name: _faker.Random.String2(length: 10),
            userId: _faker.Random.Int(min: 1, max: 100),
            password: _faker.Random.String2(length: 10));
    }
}
