#region

using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Domain.Abstractions.Clock;
using PlanningPoker.Domain.Invitations;
using PlanningPoker.Domain.Users;
using PlanningPoker.UnitTests.Common.Extensions;

#endregion

namespace PlanningPoker.UnitTests.Domain.Invitations;

public class InvitationTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    [Fact]
    public void New_InvalidData_ReturnsErrorsWithInvalidPropertyDetails()
    {
        var invitation = FakerInstance.NewInvalidInvitation();

        var errors = invitation.Errors;

        errors.Should().BeEquivalentTo([
            new { Code = "Invitation.Receiver", Message = "Provided email is not valid." },
            new { Code = "TenantId", Message = "Provided value cannot be null, empty or white space." }
        ]);
    }

    [Fact]
    public void New_ValidData_SetExpiresAtUtcTo30MinutesAfterNow()
    {
        var invitation = GetNewValidInvitation();

        var diff = (int)(invitation.ExpiresAtUtc - invitation.CreatedAtUtc).TotalMinutes;

        diff.Should().Be(InvitationConstants.ExpirationTimeInMinutes);
    }

    [Fact]
    public void New_ValidData_RegisterInvitationCreatedEvent()
    {
        var invitation = GetNewValidInvitation();

        invitation.GetDomainEvents().Should().BeEquivalentTo([
            new InvitationCreated(invitation.Id, invitation.Receiver, invitation.ExpiresAtUtc)
        ]);
    }

    [Fact]
    public void Renew_NonFinishedInvitation_RegisterEventAndRefreshExpiresAtUtc()
    {
        var dates = new Queue<DateTime>([
            DateTime.UtcNow.AddDays(-100),
            DateTime.UtcNow,
            DateTime.UtcNow
        ]);
        _dateTimeProvider.UtcNow().Returns(_ => dates.Dequeue());
        var invitation = FakerInstance.NewValidInvitation(dateTimeProvider: _dateTimeProvider);
        var oldExpiresAt = invitation.ExpiresAtUtc;
        var sentAt = invitation.SentAtUtc;

        invitation.Renew();

        using var _ = new AssertionScope();
        invitation.GetDomainEvents().Should().Contain(
            new InvitationRenewed(invitation.Id, invitation.Receiver, invitation.ExpiresAtUtc)
        );
        invitation.SentAtUtc.Should().BeAfter(sentAt);
        invitation.ExpiresAtUtc.Should().BeAfter(oldExpiresAt);
    }

    [Theory]
    [MemberData(nameof(InvitationFixture.GetAcceptedOrCancelledInvitations), MemberType = typeof(InvitationFixture))]
    public void Renew_AcceptedOrCancelledInvitation_ReturnsFinishedInvitationError(
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
    public void Accept_AcceptedOrCancelledInvitation_ReturnsFinishedInvitationError(
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
    public void Accept_ExpiredInvitation_ReturnsExpiredInvitationError()
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