using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Users.RenewInvitation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;
using PlanningPoker.UnitTests.Domain.Users.Extensions;

namespace PlanningPoker.UnitTests.Application.Users.RenewInvitation
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
        public async Task ShouldReturnValidationErrorWhenInvitationIdIsNotValid()
        {
            var command = new RenewInvitationCommand(id: 0);

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldUpdateSentAtUtcAndExpiresAtUtcDate()
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
        public async Task ShouldReturnNotFoundWhenProvidedInvitationDoesNotExists()
        {
            var command = new RenewInvitationCommand(id: _faker.ValidId());

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.RecordNotFound);
        }
    }
}
