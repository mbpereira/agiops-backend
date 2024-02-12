using Bogus;
using FluentAssertions;
using PlanningPoker.Domain.Users;
using PlanningPoker.Domain.Users.Events;

namespace PlanningPoker.UnitTests.Domain.Users
{
    public class InviteTests
    {
        private readonly Faker _faker;

        public InviteTests()
        {
            _faker = new();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abcd")]
        public void ShouldReturnExpectedErrorsWhenProvidedDataIsNotValid(string invalidEmail)
        {
            var invite = Invite.New(tenantId: _faker.Random.Int(min: 1), to: invalidEmail, _faker.PickRandom<Role>());

            var validationResult = invite.Validate();

            validationResult.Errors.Should().BeEquivalentTo(new[] { new { Code = "Invite.To" } });
        }

        [Fact]
        public void ShouldSetExpiresAtUtcTo30MinutesAfterNow()
        {
            var invite = GetValidInvite();

            invite.ExpiresAtUtc.Should().Be(invite.CreatedAtUtc.AddMinutes(30));
        }

        [Fact]
        public void ShouldRegisterInviteCreatedEvent()
        {
            var invite = GetValidInvite();

            invite.GetDomainEvents().Should().BeEquivalentTo(new[] { new InviteCreated(invite.Token, invite.To, invite.ExpiresAtUtc) });
        }

        private Invite GetValidInvite()
            => Invite.New(tenantId: _faker.Random.Int(min: 1), to: _faker.Person.Email, role: _faker.PickRandom<Role>());
    }
}
