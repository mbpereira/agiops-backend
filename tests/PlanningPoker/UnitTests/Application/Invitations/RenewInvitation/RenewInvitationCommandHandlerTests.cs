using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Invitations.RenewInvitation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Invitations;
using PlanningPoker.UnitTests.Domain.Common.Extensions;

namespace PlanningPoker.UnitTests.Application.Invitations.RenewInvitation
{
    public class RenewInvitationCommandHandlerTests
    {
        private readonly IInvitationsRepository _invitations;
        private readonly Faker _faker;
        private readonly RenewInvitationCommandHandler _handler;

        public RenewInvitationCommandHandlerTests()
        {
            _invitations = Substitute.For<IInvitationsRepository>();
            var uow = Substitute.For<IUnitOfWork>();
            uow.Invitations.Returns(_invitations);
            _faker = new();
            _handler = new RenewInvitationCommandHandler(uow);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnValidationErrorWhenInvitationIdIsNotValid()
        {
            var command = new RenewInvitationCommand(id: 0);

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Theory]
        [InlineData(InvitationStatus.Accepted)]
        [InlineData(InvitationStatus.Cancelled)]
        public async Task HandleAsync_ShouldReturnValidationErrorWhenInvitationAlreadyBeenAcceptedOrInactived(
            InvitationStatus status)
        {
            var expectedErrors = new[]
            {
                new { Code = "Invitation.Renew", Message = "This invitation has already been accepted or is inactive." }
            };
            var expectedInvitation = _faker.LoadValidInvitation(status: status);
            var command = new RenewInvitationCommand(expectedInvitation.Id.Value);
            _invitations.GetByIdAsync(Arg.Any<EntityId>()).Returns(expectedInvitation);

            var result = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            result.Status.Should().Be(CommandStatus.ValidationFailed);
            result.Details.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task HandleAsync_ShouldUpdateSentAtUtcAndExpiresAtUtcDate()
        {
            var expectedInvitation = _faker.LoadValidInvitation();
            var command = new RenewInvitationCommand(expectedInvitation.Id.Value);
            var sentAtUtc = expectedInvitation.SentAtUtc;
            var expiresAtUtc = expectedInvitation.ExpiresAtUtc;
            _invitations.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(expectedInvitation);
            await Task.Delay(TimeSpan.FromSeconds(2));

            var result = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            result.Status.Should().Be(CommandStatus.Success);
            await _invitations.Received().ChangeAsync(Arg.Is<Invitation>(i =>
                i.Id.Value == expectedInvitation.Id.Value &&
                i.SentAtUtc > sentAtUtc &&
                i.ExpiresAtUtc > expiresAtUtc));
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnNotFoundWhenProvidedInvitationDoesNotExists()
        {
            var command = new RenewInvitationCommand(id: _faker.ValidId());

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.RecordNotFound);
        }
    }
}