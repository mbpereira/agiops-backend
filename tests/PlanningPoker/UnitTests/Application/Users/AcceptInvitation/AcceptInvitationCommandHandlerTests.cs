using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Users;
using PlanningPoker.Application.Users.AcceptInvitation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;
using PlanningPoker.UnitTests.Domain.Users.Extensions;

namespace PlanningPoker.UnitTests.Application.Users.AcceptInvitation
{
    public class AcceptInvitationCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly IInvitationsRepository _invitations;
        private readonly IAccessGrantsRepository _grants;
        private readonly IUserContext _userContext;
        private readonly AcceptInvitationCommandHandler _handler;

        public AcceptInvitationCommandHandlerTests()
        {
            _faker = new();
            _grants = Substitute.For<IAccessGrantsRepository>();
            _invitations = Substitute.For<IInvitationsRepository>();
            var uow = Substitute.For<IUnitOfWork>();
            uow.Invitations.Returns(_invitations);
            uow.AccessGrants.Returns(_grants);
            _userContext = Substitute.For<IUserContext>();
            _handler = new AcceptInvitationCommandHandler(uow, _userContext);
        }

        [Fact]
        public async Task ShouldReturnErrorWhenInvitationIdIsNotValid()
        {
            var command = new AcceptInvitationCommand(invitationId: 0);

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldReturnErrorWhenInvitationHasExpired()
        {
            var command = new AcceptInvitationCommand(invitationId: _faker.ValidId());
            _invitations.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(_faker.LoadValidInvitation(expiresAtUtc: DateTime.UtcNow.AddDays(-15)));

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldReturnRecordNotFoundWhenInvitationWasNotFound()
        {
            var command = new AcceptInvitationCommand(invitationId: _faker.ValidId());

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.RecordNotFound);
        }

        [Theory]
        [InlineData(InvitationStatus.Accepted)]
        [InlineData(InvitationStatus.Inactive)]
        public async Task ShouldReturnValidationErrorWhenInvitationAlreadyBeenAcceptedOrInactived(InvitationStatus status)
        {
            var command = new AcceptInvitationCommand(invitationId: _faker.ValidId());
            _invitations.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(_faker.LoadValidInvitation(status: status));

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldReturnSuccessWhenInvitationIsAcceptedWithoutErrors()
        {
            var expectedInvitation = _faker.LoadValidInvitation();
            var updatedAt = expectedInvitation.UpdatedAtUtc.GetValueOrDefault();
            var command = new AcceptInvitationCommand(invitationId: expectedInvitation.Id.Value);
            _invitations.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(expectedInvitation);
            await Task.Delay(TimeSpan.FromSeconds(3));

            var result = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            result.Status.Should().Be(CommandStatus.Success);
            await _invitations.Received().ChangeAsync(Arg.Is<Invitation>(i =>
                InvitationStatus.Accepted.Equals(i.Status) &&
                i.Id.Value == expectedInvitation.Id.Value &&
                i.UpdatedAtUtc > updatedAt));
            expectedInvitation.GetDomainEvents().Should().BeEquivalentTo(new[]
            {
                new InvitationAccepted(expectedInvitation.Token, expectedInvitation.UpdatedAtUtc.GetValueOrDefault())
            });
        }

        [Theory]
        [InlineData(Role.Admin)]
        [InlineData(Role.Contributor)]
        public async Task ShouldRegisterUserThatAcceptedInvitationInInvitationTenant(Role role)
        {
            var userInformation = new UserInformation(Id: _faker.ValidId());
            var expectedScopes = TenantScopes.GetByRole(role);
            var expectedInvitation = _faker.LoadValidInvitation(role: role);
            var command = new AcceptInvitationCommand(expectedInvitation.Id);
            _invitations.GetByIdAsync(Arg.Any<EntityId>())
                .Returns(expectedInvitation);
            _userContext.GetCurrentUserAsync().Returns(userInformation);

            var result = await _handler.HandleAsync(command);

            await _grants.Received().AddAsync(Arg.Is<IList<AccessGrant>>(accessGrants =>
                accessGrants.All(access =>
                    access.Grant.Resource == Resources.Tenant &&
                    access.Grant.RecordId == null &&
                    access.TenantId.Value == expectedInvitation.TenantId &&
                    access.UserId.Value == userInformation.Id) &&
                expectedScopes.All(scope => accessGrants.Any(access => access.Grant.Scope == scope))));
        }
    }
}
