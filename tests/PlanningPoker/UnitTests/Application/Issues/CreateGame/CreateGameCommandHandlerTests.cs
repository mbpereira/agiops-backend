using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Issues.CreateGame;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;
using PlanningPoker.UnitTests.Domain.Users.Extensions;

namespace PlanningPoker.UnitTests.Application.Issues.CreateGame
{
    public class CreateGameCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly IUnitOfWork _uow;
        private readonly ISecurityContext _authenticationContext;
        private readonly CreateGameCommandHandler _handler;

        public CreateGameCommandHandlerTests()
        {
            _authenticationContext = Substitute.For<ISecurityContext>();
            _faker = new();
            _authenticationContext.GetSecurityInformationAsync()
                .Returns(GetSecurityInformation());
            _uow = Substitute.For<IUnitOfWork>();
            _handler = new(_uow, _authenticationContext);
        }

        private SecurityInformation GetSecurityInformation()
            => new(
                new(Id: _faker.Random.Int(min: 1)),
                new(Id: _faker.Random.Int(min: 1)));

        [Theory]
        [InlineData("", null)]
        [InlineData(null, "abcde")]
        public async Task ShouldReturnValidationFailedWhenProvidedDataIsNotValid(string invalidName, string invalidPassword)
        {
            var command = new CreateGameCommand(name: invalidName, password: invalidPassword, votingSystemId: 0);

            var commandResult = await _handler.HandleAsync(command);

            commandResult.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldReturnGeneratedIdWhenGameWasCreated()
        {
            var expectedGame = GetValidGame();
            var command = new CreateGameCommand(
                name: expectedGame.Name,
                password: expectedGame.Credentials!.Password,
                votingSystemId: _faker.ValidId());
            _uow.Games.AddAsync(Arg.Any<Game>()).Returns(expectedGame);

            var commandResult = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            commandResult.Status.Should().Be(CommandStatus.Success);
            commandResult.Data!.Id.Should().Be(expectedGame.Id.Value);
        }

        private Game GetValidGame() => _faker.NewValidGame();
    }
}
