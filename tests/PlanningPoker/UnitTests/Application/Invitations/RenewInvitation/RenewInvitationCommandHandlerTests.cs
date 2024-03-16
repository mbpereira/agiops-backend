#region

using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Invitations.RenewInvitation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Invitations;
using PlanningPoker.UnitTests.Common.Extensions;
using PlanningPoker.UnitTests.Domain.Invitations;

#endregion

namespace PlanningPoker.UnitTests.Application.Invitations.RenewInvitation;

public class RenewInvitationCommandHandlerTests
{
    private readonly InvitationCommandHandlersFixture _fixture;
    private readonly RenewInvitationCommandHandler _handler;

    public RenewInvitationCommandHandlerTests()
    {
        _fixture = new InvitationCommandHandlersFixture();
        _handler = new RenewInvitationCommandHandler(_fixture.Uow);
    }

    [Fact]
    public async Task HandleAsync_InvalidInvitationId_ReturnsValidationFailed()
    {
        var command = new RenewInvitationCommand(FakerInstance.InvalidId());

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Theory]
    [MemberData(nameof(InvitationFixture.GetAcceptedOrCancelledInvitations),
        MemberType = typeof(InvitationFixture))]
    public async Task HandleAsync_AlreadyAcceptedOrCancelledInvitation_ReturnsValidationFailed(
        Invitation finishedInvitation)
    {
        var command = new RenewInvitationCommand(finishedInvitation.Id.Value);
        _fixture.Invitations.GetByIdAsync(Arg.Any<EntityId>()).Returns(finishedInvitation);

        var result = await _handler.HandleAsync(command);

        using var _ = new AssertionScope();
        result.Status.Should().Be(CommandStatus.ValidationFailed);
        result.Errors.Should().BeEquivalentTo([
            new { Code = "Invitation.Renew", Message = "This invitation has already been accepted or is cancelled." }
        ]);
    }

    [Fact]
    public async Task HandleAsync_RenewedInvitation_UpdatesSentAndExpiresDates()
    {
        var expectedInvitation = FakerInstance.NewValidInvitation();
        var command = new RenewInvitationCommand(expectedInvitation.Id.Value);
        var sentAtUtc = expectedInvitation.SentAtUtc;
        var expiresAtUtc = expectedInvitation.ExpiresAtUtc;
        _fixture.Invitations.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(expectedInvitation);
        await Task.Delay(TimeSpan.FromSeconds(2));

        var result = await _handler.HandleAsync(command);

        using var _ = new AssertionScope();
        result.Status.Should().Be(CommandStatus.Success);
        await _fixture.Invitations.Received().ChangeAsync(Arg.Is<Invitation>(i =>
            i.Id.Value == expectedInvitation.Id.Value &&
            i.SentAtUtc > sentAtUtc &&
            i.ExpiresAtUtc > expiresAtUtc));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFoundWhenProvidedInvitationDoesNotExists()
    {
        var command = new RenewInvitationCommand(FakerInstance.ValidId());

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.RecordNotFound);
    }
}