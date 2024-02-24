#region

using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Abstractions.Security;
using PlanningPoker.Application.Games.CreateGame;
using PlanningPoker.Application.Tenants;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.UnitTests.Application.Games.CreateGame;

public class CreateGameCommandHandlerTests
{
    private readonly Faker _faker;
    private readonly IGamesRepository _games;
    private readonly CreateGameCommandHandler _handler;
    private readonly IVotingSystemsRepository _votingSystems;

    public CreateGameCommandHandlerTests()
    {
        _games = Substitute.For<IGamesRepository>();
        _votingSystems = Substitute.For<IVotingSystemsRepository>();
        var authenticationContext = Substitute.For<ISecurityContext>();
        _faker = new Faker();
        authenticationContext.GetSecurityInformationAsync()
            .Returns(GetSecurityInformation());
        var uow = Substitute.For<IUnitOfWork>();
        uow.Games.Returns(_games);
        uow.VotingSystems.Returns(_votingSystems);
        _handler = new CreateGameCommandHandler(uow, authenticationContext);
    }

    private SecurityInformation GetSecurityInformation()
    {
        return new SecurityInformation(
            new TenantInformation(FakerInstance.ValidId()),
            new UserInformation(FakerInstance.ValidId()));
    }

    [Theory]
    [InlineData("", null)]
    [InlineData(null, "abcde")]
    public async Task HandleAsync_ShouldReturnValidationFailedWhenProvidedDataIsNotValid(string invalidName,
        string invalidPassword)
    {
        var validVotingSystem = _faker.NewValidVotingSystem();
        var command = new CreateGameCommand(invalidName, password: invalidPassword,
            votingSystemId: validVotingSystem.Id.Value);
        _votingSystems.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(validVotingSystem);

        var commandResult = await _handler.HandleAsync(command);

        commandResult.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnValidationFailedWhenProvidedVotingSystemIdIsNotValid()
    {
        var command = new CreateGameCommand(_faker.Random.String2(10),
            password: _faker.Random.String2(10), votingSystemId: EntityId.Empty);

        var commandResult = await _handler.HandleAsync(command);

        commandResult.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRecordNotFoundWhenProvidedVotingSystemDoesNotExists()
    {
        var expectedGame = _faker.NewValidGame();
        var command = new CreateGameCommand(
            expectedGame.Name,
            password: expectedGame.Credentials?.Password,
            votingSystemId: _faker.ValidId());

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.RecordNotFound);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnGeneratedIdWhenGameWasCreated()
    {
        var validVotingSystem = _faker.NewValidVotingSystem();
        var expectedGame = _faker.NewValidGame(votingSystem: validVotingSystem);
        var command = new CreateGameCommand(
            expectedGame.Name,
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