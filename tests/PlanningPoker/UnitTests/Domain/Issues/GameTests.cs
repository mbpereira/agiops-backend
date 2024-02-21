using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;
using PlanningPoker.UnitTests.Domain.Users.Extensions;

namespace PlanningPoker.UnitTests.Domain.Issues
{
    public class GameTests
    {
        private readonly Faker _faker = new();

        [Fact]
        public void ShouldReturnCredentialsAsNullWhenPasswordIsNotSet()
        {
            var game = _faker.NewValidGame();

            game.Credentials.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnAutoIncrementAsIdWhenNewValidGameIsCreated()
        {
            var game = _faker.NewValidGame();

            using var _ = new AssertionScope();
            game.Id.Should().Be(EntityId.AutoIncrement());
            game.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnProvidedPassword()
        {
            var password = _faker.Random.String2(length: 25);

            var game = _faker.NewValidGame(password);

            using var _ = new AssertionScope();
            game.Credentials!.Password.Should().Be(password);
            game.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldReturnValidationErrorsWhenProvidedDataIsNotValid(string? name)
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
                    Message = "Provided value must be greater than 0."
                },
                new
                {
                    Code = "Game.UserId",
                    Message = "Provided value must be greater than 0."
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

            var game = Game.New(tenantId: 0, name!, userId: 0, password: _faker.Random.String2(length: 2), votingSystem: null!);

            using var _ = new AssertionScope();
            game.IsValid.Should().BeFalse();
            game.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public void ShouldReturnErrorWhenProvidedVotingSystemIsNotValid()
        {
            var game = _faker.NewValidGame();
            
            game.SetVotingSystem(_faker.InvalidVotingSystem());

            game.Errors.Should().BeEquivalentTo(new[]
            {
                new { Code = "Game.SetVotingSystem", Message = "Provided voting system is not valid." }
            });
        }

        [Fact]
        public void ShouldReturnErrorWhenTryingToChangeGameOwner()
        {
            var game = _faker.LoadValidGame();

            game.SetOwner(_faker.ValidId());

            game.Errors.Should().BeEquivalentTo(new[]
            {
                new { Code = "Game.UserId", Message = "You cannot change the game owner, as it has already been set." }
            });
        }

        [Fact]
        public void ShouldReturnIsValidAsTrueWhenProvidedDataIsValid()
        {
            var game = _faker.NewValidGame(password: _faker.Random.String2(length: 6));

            using var _ = new AssertionScope();
            game.IsValid.Should().BeTrue();
            game.Errors.Should().BeEmpty();
        }
    }
}
