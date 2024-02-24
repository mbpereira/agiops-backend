#region

using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.UnitTests.Domain.Games;

public class GameTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void New_ShouldReturnCredentialsAsNullWhenPasswordIsNotSet()
    {
        var game = _faker.NewValidGame();

        game.Credentials.Should().BeNull();
    }

    [Fact]
    public void New_ShouldReturnProvidedPassword()
    {
        var password = _faker.Random.String2(25);

        var game = _faker.NewValidGame(password);

        using var _ = new AssertionScope();
        game.Credentials!.Password.Should().Be(password);
        game.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void New_ProvidedInformationIsNotValid_ReturnsValidationErrors(string? name)
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

        var game = Game.New(EntityId.Empty, name!, EntityId.Empty, password: _faker.Random.String2(2),
            votingSystem: null!);

        using var _ = new AssertionScope();
        game.IsValid.Should().BeFalse();
        game.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void SetVotingSystem_ShouldReturnErrorWhenProvidedVotingSystemIsNotValid()
    {
        var game = _faker.NewValidGame();

        game.SetVotingSystem(_faker.InvalidVotingSystem());

        game.Errors.Should().BeEquivalentTo([
            new { Code = "Game.SetVotingSystem", Message = "Provided voting system is not valid." }
        ]);
    }

    [Fact]
    public void SetOwner_ShouldReturnErrorWhenTryingToChangeGameOwner()
    {
        var game = _faker.NewValidGame();

        game.SetOwner(_faker.ValidId());

        game.Errors.Should().BeEquivalentTo(new[]
        {
            new { Code = "Game.UserId", Message = "You cannot change the game owner, as it has already been set." }
        });
    }

    [Fact]
    public void New_ShouldReturnIsValidAsTrueWhenProvidedDataIsValid()
    {
        var game = _faker.NewValidGame(_faker.Random.String2(6));

        using var _ = new AssertionScope();
        game.IsValid.Should().BeTrue();
        game.Errors.Should().BeEmpty();
    }
}