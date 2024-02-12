using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;
using PlanningPoker.Domain.Users.Events;
using PlanningPoker.UnitTests.Domain.Users.Extensions;

namespace PlanningPoker.UnitTests.Domain.Users
{
    public class InviteTests
    {
        private readonly Faker _faker;

        public InviteTests()
        {
            _faker = new();
        }

        [Fact]
        public void New_ShouldReturnExpectedErrorsWhenProvidedDataIsNotValid()
        {
            var invite = _faker.NewInvalidInvite();

            var validationResult = invite.Validate();

            validationResult.Errors.Should().BeEquivalentTo(new[] { new { Code = "Invite.To" } });
        }

        [Fact]
        public void New_ShouldSetExpiresAtUtcTo30MinutesAfterNow()
        {
            var invite = GetNewValidInvite();

            var diff = (int)(invite.ExpiresAtUtc - invite.CreatedAtUtc).TotalMinutes;

            diff.Should().Be(Invite.ExpirationTimeInMinutes);
        }

        [Fact]
        public void New_ShouldRegisterInviteCreatedEvent()
        {
            var invite = GetNewValidInvite();

            invite.GetDomainEvents().Should().BeEquivalentTo(new[] { new InviteCreated(invite.Token, invite.To, invite.ExpiresAtUtc) });
        }

        [Fact]
        public async Task Renew_ShouldRegisterInviteRenewedEventAndRefreshExpiresAtUtcDate()
        {
            var invite = _faker.LoadValidInvite();
            var expiresAt = invite.ExpiresAtUtc;
            var sentAt = invite.SentAtUtc;
            await Task.Delay(TimeSpan.FromSeconds(1));

            invite.Renew();

            using var _ = new AssertionScope();
            invite.GetDomainEvents().Should().BeEquivalentTo(new[] { new InviteRenewed(invite.Token, invite.To, invite.ExpiresAtUtc) });
            invite.SentAtUtc.Should().BeAfter(sentAt);
            invite.ExpiresAtUtc.Should().BeAfter(expiresAt);
        }

        [Theory]
        [InlineData(InviteStatus.Accepted)]
        [InlineData(InviteStatus.Inactive)]
        public void Accept_ShouldThrowExceptionWhenCurrentStatusIsNotOpen(InviteStatus status)
        {
            var invite = _faker.LoadValidInvite(status: status);
            var act = () => invite.Accept();

            var ex = act.Should().Throw<DomainException>();

            ex.Which.Message.Should().Be("This invitation has already been accepted or is inactive.");
        }

        [Fact]
        public void Accept_ShouldThrowExceptionWhenInviteHasExpired()
        {
            var invite = _faker.LoadValidInvite(expiresAtUtc: DateTime.UtcNow.AddDays(-30));
            var act = () => invite.Accept();

            var ex = act.Should().Throw<DomainException>();

            ex.Which.Message.Should().Be("This invitation has expired.");
        }

        private Invite GetNewValidInvite()
            => Invite.New(tenantId: _faker.Random.Int(min: 1), to: _faker.Person.Email, role: _faker.PickRandom<Role>());
    }
}
