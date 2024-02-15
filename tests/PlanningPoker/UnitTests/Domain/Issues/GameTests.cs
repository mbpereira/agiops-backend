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
        private readonly Faker _faker;

        public GameTests()
        {
            _faker = new();
        }

        [Fact]
        public void ShouldReturnCredentialsAsNullWhenPasswordIsNotSetd()
        {
            var game = GetValidGame();

            game.Credentials.Should().BeNull();
        }

        private Game GetValidGame(string? password = null)
            => _faker.NewValidGame(password);

        [Fact]
        public void ShouldReturnAutoIncrementAsIdWhenNewGameIsCreated()
        {
            var game = GetValidGame();

            game.Id.Should().Be(EntityId.AutoIncrement());
        }

        [Fact]
        public void ShouldReturnProvidedPassword()
        {
            var password = _faker.Random.String2(length: 25);

            var game = GetValidGame(password);

            game.Credentials!.Password.Should().Be(password);
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
                    Code = "Game.name",
                    Message = "The provided string does not meet the minimum length requirement. Min length: 1."
                },
                new
                {
                    Code = "tenantId",
                    Message = "Provided value must be greater than 0."
                },
                new
                {
                    Code = "Game.userId",
                    Message = "Provided value must be greater than 0."
                },
                new
                {
                    Code = "Game.password",
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
        public void ShouldReturnIsValidAsTrueWhenProvidedDataIsValid()
        {
            var game = GetValidGame(password: _faker.Random.String2(length: 6));

            using var _ = new AssertionScope();
            game.IsValid.Should().BeTrue();
            game.Errors.Should().BeEmpty();
        }
    }
}
