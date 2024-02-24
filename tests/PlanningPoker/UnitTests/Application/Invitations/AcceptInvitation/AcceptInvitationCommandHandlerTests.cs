#region

using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Invitations.AcceptInvitation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Invitations;
using PlanningPoker.Domain.Tenants;
using PlanningPoker.Domain.Users;
using PlanningPoker.UnitTests.Domain.Invitations;

#endregion

namespace PlanningPoker.UnitTests.Application.Invitations.AcceptInvitation;

public class AcceptInvitationCommandHandlerTests
{
    private readonly InvitationCommandHandlersFixture _fixture;
    private readonly AcceptInvitationCommandHandler _handler;

    public AcceptInvitationCommandHandlerTests()
    {
        _fixture = new();
        _handler = new(_fixture.Uow, _fixture.UserContext);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnErrorWhenInvitationIdIsNotValid()
    {
        var command = new AcceptInvitationCommand(FakerInstance.InvalidId());

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnErrorWhenInvitationHasExpired()
    {
        var command = new AcceptInvitationCommand(FakerInstance.ValidId());
        _fixture.DateTimeProvider.UtcNow().Returns(DateTime.UtcNow.AddDays(-15));
        var invitation = FakerInstance.NewValidInvitation(dateTimeProvider: _fixture.DateTimeProvider);
        _fixture.Invitations.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(invitation);

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRecordNotFoundWhenInvitationWasNotFound()
    {
        var command = new AcceptInvitationCommand(FakerInstance.ValidId());

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.RecordNotFound);
    }

    [Theory]
    [MemberData(nameof(InvitationFixture.GetAcceptedOrCancelledInvitations),
        MemberType = typeof(InvitationFixture))]
    public async Task HandleAsync_ShouldReturnValidationErrorWhenInvitationAlreadyBeenAcceptedOrCancelled(
        Invitation finishedInvitation)
    {
        var command = new AcceptInvitationCommand(FakerInstance.ValidId());
        _fixture.Invitations.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(finishedInvitation);

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccessWhenInvitationIsAcceptedWithoutErrors()
    {
        var expectedInvitation = FakerInstance.NewValidInvitation();
        var updatedAt = expectedInvitation.UpdatedAtUtc.GetValueOrDefault();
        var command = new AcceptInvitationCommand(expectedInvitation.Id.Value);
        _fixture.Invitations.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(expectedInvitation);
        await Task.Delay(TimeSpan.FromSeconds(3));

        var result = await _handler.HandleAsync(command);

        using var _ = new AssertionScope();
        result.Status.Should().Be(CommandStatus.Success);
        await _fixture.Invitations.Received().ChangeAsync(Arg.Is<Invitation>(i =>
            InvitationStatus.Accepted.Equals(i.Status) &&
            i.Id.Value == expectedInvitation.Id.Value &&
            i.UpdatedAtUtc > updatedAt));
        expectedInvitation.GetDomainEvents().Should().Contain(new InvitationAccepted(expectedInvitation.Id,
            expectedInvitation.UpdatedAtUtc.GetValueOrDefault()));
    }

    [Theory]
    [InlineData(Role.Admin)]
    [InlineData(Role.Contributor)]
    public async Task HandleAsync_ShouldRegisterUserThatAcceptedInvitationInInvitationTenant(Role role)
    {
        var expectedScopes = TenantScopes.GetByRole(role);
        var expectedInvitation = FakerInstance.NewValidInvitation(role: role);
        var command = new AcceptInvitationCommand(expectedInvitation.Id);
        _fixture.Invitations.GetByIdAsync(Arg.Any<EntityId>())
            .Returns(expectedInvitation);

        _ = await _handler.HandleAsync(command);

        await _fixture.Grants.Received().AddAsync(Arg.Is<IList<AccessGrant>>(accessGrants =>
            accessGrants.All(access =>
                access.Grant.Resource == Resources.Tenant &&
                access.Grant.RecordId == null &&
                access.TenantId.Value == expectedInvitation.TenantId &&
                access.UserId.Value == _fixture.UserInformation.Id) &&
            expectedScopes.All(scope => accessGrants.Any(access => access.Grant.Scope == scope))));
    }
}