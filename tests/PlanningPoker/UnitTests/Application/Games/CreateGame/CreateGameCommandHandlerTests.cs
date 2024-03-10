#region

using FluentAssertions;
using FluentAssertions.Execution;
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
    private readonly IGamesRepository _games;
    private readonly CreateGameCommandHandler _handler;
    private readonly SecurityInformation _securityInformation;
    private readonly IVotingSystemsRepository _votingSystems;

    public CreateGameCommandHandlerTests()
    {
        _games = Substitute.For<IGamesRepository>();
        _votingSystems = Substitute.For<IVotingSystemsRepository>();
        var authenticationContext = Substitute.For<ISecurityContext>();
        _securityInformation = GetSecurityInformation();
        authenticationContext.GetSecurityInformationAsync()
            .Returns(_securityInformation);
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
    public async Task HandleAsync_InvalidDataProvided_ReturnsValidationFailed(string invalidName,
        string invalidPassword)
    {
        var validVotingSystem = FakerInstance.NewValidVotingSystem();
        var command = new CreateGameCommand(invalidName, password: invalidPassword,
            votingSystemId: validVotingSystem.Id.Value);
        _votingSystems.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(validVotingSystem);

        var commandResult = await _handler.HandleAsync(command);

        commandResult.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_InvalidVotingSystemIdProvided_ReturnsValidationFailed()
    {
        var command = new CreateGameCommand(FakerInstance.Random.String2(10),
            password: FakerInstance.Random.String2(10), votingSystemId: EntityId.Empty);

        var commandResult = await _handler.HandleAsync(command);

        commandResult.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_VotingSystemDoesNotExist_ReturnsRecordNotFound()
    {
        var expectedGame = FakerInstance.NewValidGame();
        var command = new CreateGameCommand(
            expectedGame.Name,
            password: expectedGame.Credentials?.Password,
            votingSystemId: FakerInstance.ValidId());

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.RecordNotFound);
    }

    [Fact]
    public async Task HandleAsync_SuccessfulGameCreation_ReturnsGeneratedId()
    {
        var validVotingSystem = FakerInstance.NewValidVotingSystem();
        var expectedGame = FakerInstance.NewValidGame(votingSystem: validVotingSystem,
            password: FakerInstance.Random.String2(20));
        var command = new CreateGameCommand(
            expectedGame.Name,
            password: expectedGame.Credentials?.Password,
            votingSystemId: FakerInstance.ValidId(),
            teamId: FakerInstance.ValidId());
        _games.AddAsync(Arg.Any<Game>()).Returns(expectedGame);
        _votingSystems.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(validVotingSystem);

        var commandResult = await _handler.HandleAsync(command);

        using var _ = new AssertionScope();
        await _games.Received().AddAsync(Arg.Is<Game>(g =>
            g.TeamId!.Value == command.TeamId &&
            g.GradeDetails == validVotingSystem.GradeDetails &&
            g.TenantId.Value == _securityInformation.Tenant.Id &&
            g.UserId.Value == _securityInformation.User.Id &&
            g.Credentials!.Password == command.Password));
        commandResult.Status.Should().Be(CommandStatus.Success);
        commandResult.Data!.Id.Should().Be(expectedGame.Id.Value);
    }
}