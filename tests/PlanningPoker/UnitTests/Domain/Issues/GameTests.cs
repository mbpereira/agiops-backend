using Bogus;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;

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
        public void ShouldReturnCredentialsAsNullWhenPasswordIsNotDefined()
        {
            var game = GetGame();

            game.Credentials.Should().BeNull();
        }

        private Game GetGame(string? password = null)
            => Game.New(_faker.Random.Int(min: 1), name: _faker.Random.String2(length: 5), userId: _faker.Random.Int(min: 1), password);

        [Fact]
        public void ShouldReturnAutoIncrementAsIdWhenNewGameIsCreated()
        {
            var game = GetGame();

            game.Id.Should().Be(EntityId.AutoIncrement());
        }

        [Fact]
        public void ShouldReturnProvidedPassword()
        {
            var password = _faker.Random.String2(length: 25);

            var game = GetGame(password);

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
                    Code = "tenantId.value",
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
                }
            };

            var game = Game.New(tenantId: 0, name!, userId: 0, password: _faker.Random.String2(length: 2));

            using var _ = new AssertionScope();
            game.IsValid.Should().BeFalse();
            game.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public void ShouldReturnIsValidAsTrueWhenProvidedDataIsValid()
        {
            var game = GetGame(password: _faker.Random.String2(length: 6));

            using var _ = new AssertionScope();
            game.IsValid.Should().BeTrue();
            game.Errors.Should().BeEmpty();
        }
    }
}
