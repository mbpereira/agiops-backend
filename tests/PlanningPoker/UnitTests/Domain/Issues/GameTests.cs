using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
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
            var game = Game.New(_faker.Random.String2(length: 5), _faker.Random.Int());

            game.Credentials.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnProvidedPassword()
        {
            var password = _faker.Random.String2(length: 25);

            var game = Game.New(_faker.Random.String2(length: 5), _faker.Random.Int(), password);

            game.Credentials!.Password.Should().Be(password);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldReturnValidationErrorsWhenProvidedDataIsNotValid(string? name)
        {
            var game = Game.New(name!, 0, _faker.Random.String2(length: 2));
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
            var game = Game.New(_faker.Random.String2(length: 1), _faker.Random.Int(), _faker.Random.String2(length: 6));

            var validationResult = game.Validate();

            validationResult.IsValid.Should().BeTrue();
        }
    }
}
