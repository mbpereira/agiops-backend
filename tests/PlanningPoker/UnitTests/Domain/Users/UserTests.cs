using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.UnitTests.Domain.Users
{
    public class UserTests
    {
        private static readonly Faker Faker = new();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void New_ShouldReturnExpectedErrors(string invalidName)
        {
            var expectedErrors = new[]
            {
                new
                {
                    Code = "User.Name",
                    Message = "The provided string does not meet the minimum length requirement. Min length: 3."
                }
            };

            var user = User.New(invalidName, email: Faker.Internet.Email());

            using var _ = new AssertionScope();
            user.Errors.Should().BeEquivalentTo(expectedErrors);
            user.IsValid.Should().BeFalse();
        }

        [Fact]
        public void NewGuest_ShouldSetEmailAsNullAndSetSessionIdWhenCreatingGuest()
        {
            var user = User.NewGuest(name: Faker.Random.String2(length: 10));

            using var _ = new AssertionScope();
            user.Email.Should().BeNull();
            user.Guest!.SessionId.Should().NotBeNull();
            user.IsGuest.Should().BeTrue();
        }

        [Fact]
        public void New_ShouldSetGuestAsNullWhenCreatingUser()
        {
            var email = Faker.Internet.Email();

            var user = User.New(name: Faker.Random.String2(length: 10), email);

            using var _ = new AssertionScope();
            user.Guest.Should().BeNull();
            user.IsGuest.Should().BeFalse();
            user.Email!.Value.Should().Be(email);
        }

        [Fact]
        public void New_ShouldReturnsErrorWhenEmailIsNotSet()
        {
            var name = Faker.Random.String2(length: 5);

            var user = User.New(name: name, email: null!);

            user.Errors.Should().BeEquivalentTo(new[]
            {
                new { Code = "User.IdentifyUser", Message = "A valid email or session id must be defined." }
            });
        }
    }
}