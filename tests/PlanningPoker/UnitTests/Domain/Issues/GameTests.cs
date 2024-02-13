using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
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
            => Game.New(_faker.Random.Int(min: 1), name: _faker.Random.String2(length: 5), userId: _faker.Random.Int(), password);

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
            var game = Game.New(tenantId: 0, name!, userId: 0, password: _faker.Random.String2(length: 2));
            var expectedErrors = new[]
            {
                new { Code = "Game.Name" },
                new { Code = "Game.Credentials.Password" }
            };

            var validationResult = game.Validate();

            using var _ = new AssertionScope();
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public void ShouldReturnIsValidAsTrueWhenProvidedDataIsValid()
        {
            var game = GetGame(password: _faker.Random.String2(length: 6));

            var validationResult = game.Validate();

            validationResult.IsValid.Should().BeTrue();
        }
    }
}
