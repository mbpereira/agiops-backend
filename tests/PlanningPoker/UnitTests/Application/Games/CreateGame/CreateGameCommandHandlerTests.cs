using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Abstractions.Security;
using PlanningPoker.Application.Games.CreateGame;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;
using PlanningPoker.UnitTests.Domain.Common.Extensions;

namespace PlanningPoker.UnitTests.Application.Games.CreateGame
{
    public class CreateGameCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly IVotingSystemsRepository _votingSystems;
        private readonly IGamesRepository _games;
        private readonly CreateGameCommandHandler _handler;

        public CreateGameCommandHandlerTests()
        {
            _games = Substitute.For<IGamesRepository>();
            _votingSystems = Substitute.For<IVotingSystemsRepository>();
            var authenticationContext = Substitute.For<ISecurityContext>();
            _faker = new();
            authenticationContext.GetSecurityInformationAsync()
                .Returns(GetSecurityInformation());
            var uow = Substitute.For<IUnitOfWork>();
            uow.Games.Returns(_games);
            uow.VotingSystems.Returns(_votingSystems);
            _handler = new(uow, authenticationContext);
        }

        private SecurityInformation GetSecurityInformation()
            => new(
                new(Id: _faker.Random.Int(min: 1)),
                new(Id: _faker.Random.Int(min: 1)));

        [Theory]
        [InlineData("", null)]
        [InlineData(null, "abcde")]
        public async Task HandleAsync_ShouldReturnValidationFailedWhenProvidedDataIsNotValid(string invalidName,
            string invalidPassword)
        {
            var validVotingSystem = _faker.LoadValidVotingSystem();
            var command = new CreateGameCommand(name: invalidName, password: invalidPassword,
                votingSystemId: validVotingSystem.Id.Value);
            _votingSystems.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(validVotingSystem);

            var commandResult = await _handler.HandleAsync(command);

            commandResult.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnValidationFailedWhenProvidedVotingSystemIdIsNotValid()
        {
            var command = new CreateGameCommand(name: _faker.Random.String2(length: 10),
                password: _faker.Random.String2(length: 10), votingSystemId: 0);

            var commandResult = await _handler.HandleAsync(command);

            commandResult.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnRecordNotFoundWhenProvidedVotingSystemDoesNotExists()
        {
            var expectedGame = _faker.NewValidGame();
            var command = new CreateGameCommand(
                name: expectedGame.Name,
                password: expectedGame.Credentials?.Password,
                votingSystemId: _faker.ValidId());

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.RecordNotFound);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnGeneratedIdWhenGameWasCreated()
        {
            var validVotingSystem = _faker.LoadValidVotingSystem();
            var expectedGame = _faker.NewValidGame(votingSystem: validVotingSystem);
            var command = new CreateGameCommand(
                name: expectedGame.Name,
                password: expectedGame.Credentials?.Password,
                votingSystemId: _faker.ValidId());
            _games.AddAsync(Arg.Any<Game>()).Returns(expectedGame);
            _votingSystems.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(validVotingSystem);

            var commandResult = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            commandResult.Status.Should().Be(CommandStatus.Success);
            commandResult.Data!.Id.Should().Be(expectedGame.Id.Value);
        }
    }
}