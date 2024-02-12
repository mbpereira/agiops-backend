using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
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
            var invite = GetNewValidInvite();

            invite.ExpiresAtUtc.Should().Be(invite.CreatedAtUtc.AddMinutes(30));
        }

        [Fact]
        public void ShouldRegisterInviteCreatedEvent()
        {
            var invite = GetNewValidInvite();

            invite.GetDomainEvents().Should().BeEquivalentTo(new[] { new InviteCreated(invite.Token, invite.To, invite.ExpiresAtUtc) });
        }

        [Fact]
        public async Task ShouldRegisterInviteRenewedEventAndRefreshExpiresAtUtcDate()
        {
            var invite = LoadValidInvite();
            var expiresAt = invite.ExpiresAtUtc;
            var sentAt = invite.SentAtUtc;
            await Task.Delay(TimeSpan.FromSeconds(1));

            invite.Renew();

            using var _ = new AssertionScope();
            invite.GetDomainEvents().Should().BeEquivalentTo(new[] { new InviteRenewed(invite.Token, invite.To, invite.ExpiresAtUtc) });
            invite.SentAtUtc.Should().BeAfter(sentAt);
            invite.ExpiresAtUtc.Should().BeAfter(expiresAt);
        }

        private Invite GetNewValidInvite()
            => Invite.New(tenantId: _faker.Random.Int(min: 1), to: _faker.Person.Email, role: _faker.PickRandom<Role>());

        private Invite LoadValidInvite()
            => Invite.Load(id: _faker.Random.Int(min: 1), tenantId: _faker.Random.Int(min: 1), to: _faker.Person.Email, role: _faker.PickRandom<Role>());
    }
}
