using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.UnitTests.Domain.Users
{
    public class UserTests
    {
        private static readonly Faker _faker = new();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldReturnExpectedErrors(string invalidName)
        {
            var user = User.New(invalidName, email: _faker.Internet.Email());
            var expectedErrors = new[]
            {
                new { Code = "User.Name" }
            };

            var validationResult = user.Validate();

            using var _ = new AssertionScope();
            validationResult.Errors.Should().BeEquivalentTo(expectedErrors);
            validationResult.Success.Should().BeFalse();
        }

        [Fact]
        public void ShouldSetEmailAsNullAndSetSessionIdWhenCreatingGuest()
        {
            var user = User.NewGuest(name: _faker.Random.String2(length: 10));

            using var _ = new AssertionScope();
            user.Email.Should().BeNull();
            user.Guest!.SessionId.Should().NotBeNull();
            user.IsGuest.Should().BeTrue();
        }

        [Fact]
        public void ShouldSetGuestAsNullWhenCreatingUser()
        {
            var email = _faker.Internet.Email();

            var user = User.New(name: _faker.Random.String2(length: 10), email);

            using var _ = new AssertionScope();
            user.Guest.Should().BeNull();
            user.IsGuest.Should().BeFalse();
            user.Email!.Value.Should().Be(email);
        }

        [Fact]
        public void ShouldThrowExceptionWhenEmailIsNotSet()
        {
            var name = _faker.Random.String2(length: 5);
            var act = () => User.New(name: name, email: null!);

            var ex = act.Should().Throw<DomainException>();

            ex.Which.Message.Should().Be("Email or Session Id must be defined.");
        }
    }
}
