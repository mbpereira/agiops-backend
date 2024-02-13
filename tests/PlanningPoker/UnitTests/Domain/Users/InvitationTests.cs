using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;
using PlanningPoker.Domain.Users.Events;
using PlanningPoker.UnitTests.Domain.Users.Extensions;

namespace PlanningPoker.UnitTests.Domain.Users
{
    public class InvitationTests
    {
        private readonly Faker _faker;

        public InvitationTests()
        {
            _faker = new();
        }

        [Fact]
        public void New_ShouldReturnExpectedErrorsWhenProvidedDataIsNotValid()
        {
            var invitation = _faker.NewInvalidInvitation();

            var validationResult = invitation.Validate();

            validationResult.Errors.Should().BeEquivalentTo(new[] { new { Code = "Invitation.To" } });
        }

        [Fact]
        public void New_ShouldSetExpiresAtUtcTo30MinutesAfterNow()
        {
            var invitation = GetNewValidInvitation();

            var diff = (int)(invitation.ExpiresAtUtc - invitation.CreatedAtUtc).TotalMinutes;

            diff.Should().Be(Invitation.ExpirationTimeInMinutes);
        }

        [Fact]
        public void New_ShouldRegisterInvitationCreatedEvent()
        {
            var invitation = GetNewValidInvitation();

            invitation.GetDomainEvents().Should().BeEquivalentTo(new[] { new InvitationCreated(invitation.Token, invitation.To, invitation.ExpiresAtUtc) });
        }

        [Fact]
        public async Task Renew_ShouldRegisterInvitationRenewedEventAndRefreshExpiresAtUtcDate()
        {
            var invitation = _faker.LoadValidInvitation();
            var expiresAt = invitation.ExpiresAtUtc;
            var sentAt = invitation.SentAtUtc;
            await Task.Delay(TimeSpan.FromSeconds(1));

            invitation.Renew();

            using var _ = new AssertionScope();
            invitation.GetDomainEvents().Should().BeEquivalentTo(new[] { new InvitationRenewed(invitation.Token, invitation.To, invitation.ExpiresAtUtc) });
            invitation.SentAtUtc.Should().BeAfter(sentAt);
            invitation.ExpiresAtUtc.Should().BeAfter(expiresAt);
        }

        [Theory]
        [InlineData(InvitationStatus.Accepted)]
        [InlineData(InvitationStatus.Inactive)]
        public void Accept_ShouldThrowExceptionWhenCurrentStatusIsNotOpen(InvitationStatus status)
        {
            var invitation = _faker.LoadValidInvitation(status: status);
            var act = () => invitation.Accept();

            var ex = act.Should().Throw<DomainException>();

            ex.Which.Message.Should().Be("This invitation has already been accepted or is inactive.");
        }

        [Fact]
        public void Accept_ShouldThrowExceptionWhenInvitationHasExpired()
        {
            var invitation = _faker.LoadValidInvitation(expiresAtUtc: DateTime.UtcNow.AddDays(-30));
            var act = () => invitation.Accept();

            var ex = act.Should().Throw<DomainException>();

            ex.Which.Message.Should().Be("This invitation has expired.");
        }

        private Invitation GetNewValidInvitation()
            => Invitation.New(tenantId: _faker.Random.Int(min: 1), to: _faker.Person.Email, role: _faker.PickRandom<Role>());
    }
}
