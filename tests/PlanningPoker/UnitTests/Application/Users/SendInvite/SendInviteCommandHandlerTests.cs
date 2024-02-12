using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Users;
using PlanningPoker.Application.Users.SendInvite;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;
using PlanningPoker.UnitTests.Domain.Users.Extensions;

namespace PlanningPoker.UnitTests.Application.Users.SendInvite
{
    public class SendInviteCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly ITenantContext _tenantContext;
        private readonly IInvitesRepository _invites;
        private readonly TenantInformation _tenant;
        private readonly SendInviteCommandHandler _handler;

        public SendInviteCommandHandlerTests()
        {
            _faker = new();
            _tenant = new TenantInformation(Id: _faker.Random.Int(min: 1));
            _invites = Substitute.For<IInvitesRepository>();
            _tenantContext = Substitute.For<ITenantContext>();
            _tenantContext.GetCurrentTenantAsync().Returns(_tenant);
            var uow = Substitute.For<IUnitOfWork>();
            uow.Invites.Returns(_invites);
            _handler = new SendInviteCommandHandler(uow, _tenantContext);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("abc")]
        public async Task ShouldReturnValidationErrorWhenProvidedDataIsNotValid(string invalidEmail)
        {
            var command = new SendInviteCommand(to: invalidEmail, _faker.PickRandom<Role>());

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task ShouldAddInviteAndReturnsSuccess()
        {
            var expectedInvite = _faker.ValidInvite();
            var command = new SendInviteCommand(expectedInvite.To.Value, _faker.PickRandom<Role>());
            _invites.AddAsync(Arg.Any<Invite>())
                .Returns(expectedInvite);

            var result = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            result.Status.Should().Be(CommandStatus.Success);
            await _invites.Received().AddAsync(Arg.Is<Invite>(i =>
                i.To.Value == expectedInvite.To.Value &&
                i.TenantId.Value == expectedInvite.TenantId.Value));
        }
    }
}
