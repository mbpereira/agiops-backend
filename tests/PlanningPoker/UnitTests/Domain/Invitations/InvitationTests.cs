#region

using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions.Clock;
using PlanningPoker.Domain.Invitations;
using PlanningPoker.Domain.Users;

#endregion

namespace PlanningPoker.UnitTests.Domain.Invitations;

public class InvitationTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    [Fact]
    public void New_ShouldReturnExpectedErrorsWhenProvidedDataIsNotValid()
    {
        var invitation = FakerInstance.NewInvalidInvitation();

        var errors = invitation.Errors;

        errors.Should().BeEquivalentTo([
            new { Code = "Invitation.Receiver", Message = "Provided email is not valid." },
            new { Code = "TenantId", Message = "Provided value cannot be null, empty or white space." }
        ]);
    }

    [Fact]
    public void New_ShouldSetExpiresAtUtcTo30MinutesAfterNow()
    {
        var invitation = GetNewValidInvitation();

        var diff = (int)(invitation.ExpiresAtUtc - invitation.CreatedAtUtc).TotalMinutes;

        diff.Should().Be(InvitationConstants.ExpirationTimeInMinutes);
    }

    [Fact]
    public void New_ShouldRegisterInvitationCreatedEvent()
    {
        var invitation = GetNewValidInvitation();

        invitation.GetDomainEvents().Should().BeEquivalentTo([
            new InvitationCreated(invitation.Id, invitation.Receiver, invitation.ExpiresAtUtc)
        ]);
    }

    [Fact]
    public async Task Renew_ShouldRegisterInvitationRenewedEventAndRefreshExpiresAtUtcDate()
    {
        var invitation = FakerInstance.NewValidInvitation();
        var expiresAt = invitation.ExpiresAtUtc;
        var sentAt = invitation.SentAtUtc;
        await Task.Delay(TimeSpan.FromSeconds(1));

        invitation.Renew();

        using var _ = new AssertionScope();
        invitation.GetDomainEvents().Should().Contain(
            new InvitationRenewed(invitation.Id, invitation.Receiver, invitation.ExpiresAtUtc)
        );
        invitation.SentAtUtc.Should().BeAfter(sentAt);
        invitation.ExpiresAtUtc.Should().BeAfter(expiresAt);
    }

    [Theory]
    [MemberData(nameof(InvitationFixture.GetAcceptedOrCancelledInvitations), MemberType = typeof(InvitationFixture))]
    public void Renew_ShouldReturnFinalizedInvitationErrorWhenTryingToRenewAnAcceptedOrInactiveInvitation(
        Invitation finishedInvitation)
    {
        var expectedErrors = new[]
        {
            new
            {
                Code = "Invitation.Renew",
                Message = "This invitation has already been accepted or is cancelled."
            }
        };

        finishedInvitation.Renew();

        using var _ = new AssertionScope();
        finishedInvitation.IsValid.Should().BeFalse();
        finishedInvitation.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Theory]
    [MemberData(nameof(InvitationFixture.GetAcceptedOrCancelledInvitations), MemberType = typeof(InvitationFixture))]
    public void Accept_ShouldReturnFinalizedInvitationErrorWhenTryingToAcceptAnAcceptedOrInactiveInvitation(
        Invitation finishedInvitation)
    {
        var expectedErrors = new[]
        {
            new
            {
                Code = "Invitation.Accept",
                Message = "This invitation has already been accepted or is cancelled."
            }
        };

        finishedInvitation.Accept();

        using var _ = new AssertionScope();
        finishedInvitation.IsValid.Should().BeFalse();
        finishedInvitation.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public void Accept_ShouldReturnExpiredInvitationErrorWhenTryingToAcceptExpiredInvitation()
    {
        var expectedErrors = new[]
        {
            new
            {
                Code = "Invitation.Accept",
                Message = "This invitation has expired."
            }
        };
        _dateTimeProvider.UtcNow().Returns(DateTime.Now.AddDays(-100));
        var invitation = FakerInstance.NewValidInvitation(dateTimeProvider: _dateTimeProvider);

        invitation.Accept();

        using var _ = new AssertionScope();
        invitation.IsValid.Should().BeFalse();
        invitation.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    private Invitation GetNewValidInvitation()
    {
        return Invitation.New(FakerInstance.ValidId(), FakerInstance.Person.Email,
            FakerInstance.PickRandom<Role>(), DefaultDateTimeProvider.Instance);
    }
}