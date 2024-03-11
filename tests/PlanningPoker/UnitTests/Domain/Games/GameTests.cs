#region

using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Games;
using PlanningPoker.UnitTests.Common.Extensions;

#endregion

namespace PlanningPoker.UnitTests.Domain.Games;

public class GameTests
{
    [Fact]
    public void New_PasswordNotSet_ReturnsNullCredentials()
    {
        var game = FakerInstance.NewValidGame();

        game.Credentials.Should().BeNull();
    }

    [Fact]
    public void New_PasswordSet_ReturnsCredentialsWithPassword()
    {
        var password = FakerInstance.Random.String2(25);

        var game = FakerInstance.NewValidGame(password);

        using var _ = new AssertionScope();
        game.Credentials!.Password.Should().Be(password);
        game.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void New_InvalidData_ReturnsValidationErrors(string? name)
    {
        var expectedErrors = new[]
        {
            new
            {
                Code = "Game.Name",
                Message = "The provided string does not meet the minimum length requirement. Min length: 1."
            },
            new
            {
                Code = "TenantId",
                Message = "Provided value cannot be null, empty or white space."
            },
            new
            {
                Code = "Game.UserId",
                Message = "Provided value cannot be null, empty or white space."
            },
            new
            {
                Code = "Game.Password",
                Message = "The provided string does not meet the minimum length requirement. Min length: 6."
            },
            new
            {
                Code = "Game.SetVotingSystem",
                Message = "Provided voting system is not valid."
            }
        };

        var game = Game.New(EntityId.Empty, name!, EntityId.Empty, password: FakerInstance.Random.String2(2),
            votingSystem: null!);

        using var _ = new AssertionScope();
        game.IsValid.Should().BeFalse();
        game.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void SetVotingSystem_InvalidVotingSystem_ReturnsError()
    {
        var game = FakerInstance.NewValidGame();

        game.SetVotingSystem(FakerInstance.InvalidVotingSystem());

        game.Errors.Should().BeEquivalentTo([
            new { Code = "Game.SetVotingSystem", Message = "Provided voting system is not valid." }
        ]);
    }

    [Fact]
    public void SetOwner_AttemptToChangeOwner_ReturnsError()
    {
        var game = FakerInstance.NewValidGame();

        game.SetOwner(FakerInstance.ValidId());

        game.Errors.Should().BeEquivalentTo([
            new { Code = "Game.UserId", Message = "You cannot change the game owner, as it has already been set." }
        ]);
    }

    [Fact]
    public void New_ValidData_ReturnsIsValidTrue()
    {
        var game = FakerInstance.NewValidGame(FakerInstance.Random.String2(6));

        using var _ = new AssertionScope();
        game.IsValid.Should().BeTrue();
        game.Errors.Should().BeEmpty();
    }

    [Fact]
    public void SetTeamId_PresentTeamId_ReturnsNotNullTeamId()
    {
        var game = FakerInstance.NewValidGame();
        var teamId = Guid.NewGuid().ToString();

        game.SetTeamId(teamId);

        game.TeamId!.Value.IsPresent().Should().BeTrue();
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(null)]
    public void SetTeamId_NullEmptyOrWhiteSpaceTeamId_ReturnsNullTeamId(string invalidTeamId)
    {
        var game = FakerInstance.NewValidGame();

        game.SetTeamId(invalidTeamId);

        game.TeamId.Should().BeNull();
    }
}