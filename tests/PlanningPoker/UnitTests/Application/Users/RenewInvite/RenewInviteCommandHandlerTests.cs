using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Users.RenewInvite;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;
using PlanningPoker.UnitTests.Domain.Users.Extensions;

namespace PlanningPoker.UnitTests.Application.Users.RenewInvite
{
    public class RenewInviteCommandHandlerTests
    {
        private readonly IInvitesRepository _invites;
        private readonly Faker _faker;
        private readonly RenewInviteCommandHandler _handler;

        public RenewInviteCommandHandlerTests()
        {
            _invites = Substitute.For<IInvitesRepository>();
            var uow = Substitute.For<IUnitOfWork>();
            uow.Invites.Returns(_invites);
            _faker = new();
            _handler = new RenewInviteCommandHandler(uow);
        }

        [Fact]
        public async Task ShouldReturnValidationErrorWhenInviteIdIsNotValid()
        {
            var command = new RenewInviteCommand(id: 0);

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldUpdateSentAtUtcAndExpiresAtUtcDate()
        {
            var expectedInvite = _faker.ValidInvite();
            var command = new RenewInviteCommand(expectedInvite.Id.Value);
            var sentAtUtc = expectedInvite.SentAtUtc;
            var expiresAtUtc = expectedInvite.ExpiresAtUtc;
            _invites.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(expectedInvite);
            await Task.Delay(TimeSpan.FromSeconds(2));

            var result = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            result.Status.Should().Be(CommandStatus.Success);
            await _invites.Received().ChangeAsync(Arg.Is<Invite>(i =>
                i.Id.Value == expectedInvite.Id.Value &&
                i.SentAtUtc > sentAtUtc &&
                i.ExpiresAtUtc > expiresAtUtc));
        }

        [Fact]
        public async Task ShouldReturnNotFoundWhenProvidedInviteDoesNotExists()
        {
            var command = new RenewInviteCommand(id: _faker.ValidId());

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.RecordNotFound);
        }
    }
}
