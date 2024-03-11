#region

using FluentAssertions;
using FluentAssertions.Execution;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Invitations.SendInvitation;
using PlanningPoker.Domain.Invitations;
using PlanningPoker.Domain.Users;
using PlanningPoker.UnitTests.Common.Extensions;

#endregion

namespace PlanningPoker.UnitTests.Application.Invitations.SendInvitation;

public class SendInvitationCommandHandlerTests
{
    private readonly InvitationCommandHandlersFixture _fixture;
    private readonly SendInvitationCommandHandler _handler;

    public SendInvitationCommandHandlerTests()
    {
        _fixture = new InvitationCommandHandlersFixture();
        _handler = new SendInvitationCommandHandler(_fixture.Uow, _fixture.TenantContext, _fixture.DateTimeProvider);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("abc")]
    public async Task HandleAsync_InvalidData_ReturnsValidationFailed(string invalidEmail)
    {
        var command = new SendInvitationCommand(invalidEmail, FakerInstance.PickRandom<Role>());

        var result = await _handler.HandleAsync(command);

        result.Status.Should().Be(CommandStatus.ValidationFailed);
    }

    [Fact]
    public async Task HandleAsync_ValidData_CreateInvitationAndReturnsSuccess()
    {
        var expectedInvitation = FakerInstance.NewValidInvitation(_fixture.TenantInformation.Id);
        var command = new SendInvitationCommand(expectedInvitation.Receiver.Value, expectedInvitation.Role);
        _fixture.Invitations.AddAsync(Arg.Any<Invitation>())
            .Returns(expectedInvitation);

        var result = await _handler.HandleAsync(command);

        using var _ = new AssertionScope();
        result.Status.Should().Be(CommandStatus.Success);
        await _fixture.Invitations.Received().AddAsync(Arg.Is<Invitation>(i =>
            i.Receiver.Value == command.To &&
            i.TenantId.Value == _fixture.TenantInformation.Id &&
            i.Role == command.Role));
        expectedInvitation.GetDomainEvents().Should().Contain(new InvitationCreated(expectedInvitation.Id,
            expectedInvitation.Receiver, expectedInvitation.ExpiresAtUtc));
    }
}