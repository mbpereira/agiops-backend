using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Users;
using PlanningPoker.Application.Users.SendInvitation;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;
using PlanningPoker.UnitTests.Domain.Users.Extensions;

namespace PlanningPoker.UnitTests.Application.Users.SendInvitation
{
    public class SendInvitationCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly IInvitationsRepository _invitations;
        private readonly TenantInformation _tenant;
        private readonly SendInvitationCommandHandler _handler;

        public SendInvitationCommandHandlerTests()
        {
            _faker = new();
            _tenant = new TenantInformation(Id: _faker.Random.Int(min: 1));
            _invitations = Substitute.For<IInvitationsRepository>();
            var tenantContext = Substitute.For<ITenantContext>();
            tenantContext.GetCurrentTenantAsync().Returns(_tenant);
            var uow = Substitute.For<IUnitOfWork>();
            uow.Invitations.Returns(_invitations);
            _handler = new SendInvitationCommandHandler(uow, tenantContext);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("abc")]
        public async Task HandleAsync_ShouldReturnValidationErrorWhenProvidedDataIsNotValid(string invalidEmail)
        {
            var command = new SendInvitationCommand(to: invalidEmail, _faker.PickRandom<Role>());

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task HandleAsync_ShouldAddInvitationAndReturnsSuccess()
        {
            var expectedInvitation = _faker.LoadValidInvitation(tenantId: _tenant.Id);
            var command = new SendInvitationCommand(expectedInvitation.Receiver.Value, expectedInvitation.Role);
            _invitations.AddAsync(Arg.Any<Invitation>())
                .Returns(expectedInvitation);

            var result = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            result.Status.Should().Be(CommandStatus.Success);
            await _invitations.Received().AddAsync(Arg.Is<Invitation>(i =>
                i.Receiver.Value == expectedInvitation.Receiver.Value &&
                i.TenantId.Value == expectedInvitation.TenantId.Value &&
                i.Role == expectedInvitation.Role));
        }
    }
}