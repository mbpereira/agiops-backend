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
            validationResult.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ShouldDefineEmailAsNullAndDefineSessionIdWhenCreatingGuest()
        {
            var user = User.NewGuest(name: _faker.Random.String2(length: 10));

            using var _ = new AssertionScope();
            user.Email.Should().BeNull();
            user.Guest!.SessionId.Should().NotBeNull();
            user.IsGuest.Should().BeTrue();
        }

        [Fact]
        public void ShouldDefineGuestAsNullWhenCreatingUser()
        {
            var email = _faker.Internet.Email();

            var user = User.New(name: _faker.Random.String2(length: 10), email);

            using var _ = new AssertionScope();
            user.Guest.Should().BeNull();
            user.IsGuest.Should().BeFalse();
            user.Email!.Value.Should().Be(email);
        }

        [Fact]
        public void ShouldDefineEmailWhenLoadingNonGuestUser()
        {
            var email = _faker.Internet.Email();

            var user = User.Load(id: _faker.Random.Int(), name: _faker.Random.String2(length: 10), email, sessionId: null);

            using var _ = new AssertionScope();
            user.Email!.Value.Should().Be(email);
            user.Guest.Should().BeNull();
            user.IsGuest.Should().BeFalse();
        }

        [Fact]
        public void ShouldDefineSessionWhenLoadingGuestUser()
        {
            var session = Guid.NewGuid().ToString();

            var user = User.Load(id: _faker.Random.Int(), name: _faker.Random.String2(length: 10), email: null, sessionId: session);

            using var _ = new AssertionScope();
            user.Email.Should().BeNull();
            user.Guest!.SessionId.Should().Be(session);
            user.IsGuest.Should().BeTrue();
        }

        [Fact]
        public void ShouldThrowExceptionWhenEmailAndSessionIdIsNotDefined()
        {
            var name = _faker.Random.String2(length: 5);
            var act = () => User.Load(id: _faker.Random.Int(), name: name, email: null!, sessionId: null!);

            var ex = act.Should().Throw<DomainException>();

            ex.Which.Message.Should().Be("Email or Session Id must be defined.");
        }

        [Fact]
        public void ShouldThrowExceptionWhenEmailIsNotDefined()
        {
            var name = _faker.Random.String2(length: 5);
            var act = () => User.New(name: name, email: null!);

            var ex = act.Should().Throw<DomainException>();

            ex.Which.Message.Should().Be("Email or Session Id must be defined.");
        }
    }
}
