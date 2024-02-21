using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Users;
using PlanningPoker.Application.Users.CreateTenant;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.UnitTests.Application.Users.CreateTenant
{
    public class CreateTenantCommandHandlerTests
    {
        private readonly Faker _faker;
        private readonly ITenantsRepository _tenants;
        private readonly IAccessGrantsRepository _accessGrants;
        private readonly IUserContext _userContext;
        private readonly CreateTenantCommandHandler _handler;

        public CreateTenantCommandHandlerTests()
        {
            _faker = new();
            _userContext = Substitute.For<IUserContext>();
            _tenants = Substitute.For<ITenantsRepository>();
            _accessGrants = Substitute.For<IAccessGrantsRepository>();
            var uow = Substitute.For<IUnitOfWork>();
            uow.Tenants.Returns(_tenants);
            uow.AccessGrants.Returns(_accessGrants);
            _handler = new CreateTenantCommandHandler(uow, _userContext);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ab")]
        public async Task HandleAsync_ShouldReturnValidationErrorWhenProvidedDataIsNotValid(string invalidName)
        {
            var command = new CreateTenantCommand(invalidName);

            var result = await _handler.HandleAsync(command);

            result.Status.Should().Be(CommandStatus.ValidationFailed);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnSuccessAndSetAllAvailableTenantPermissionsToCurrentUser()
        {
            var expectedUser = new UserInformation(Id: _faker.Random.Int(min: 1));
            var expectedTenant = Tenant.Load(id: _faker.Random.Int(min: 1), name: _faker.Random.String2(length: 3));
            var command = new CreateTenantCommand(name: expectedTenant.Name);
            _tenants.AddAsync(Arg.Any<Tenant>())
                .Returns(expectedTenant);
            _userContext.GetCurrentUserAsync()
                .Returns(expectedUser);

            var result = await _handler.HandleAsync(command);

            using var _ = new AssertionScope();
            result.Status.Should().Be(CommandStatus.Success);
            await _accessGrants.Received().AddAsync(Arg.Is<IList<AccessGrant>>(accessGrants =>
                accessGrants.Select(grant => grant.Grant.Resource).All(resource => resource == Resources.Tenant) &&
                TenantScopes.GetByRole(Role.Admin)
                    .All(scope => accessGrants.Any(accessGrant => accessGrant.Grant.Scope == scope)) &&
                accessGrants.All(accessGrant =>
                    accessGrant.TenantId.Value == expectedTenant.Id &&
                    accessGrant.UserId.Value == expectedUser.Id)
            ));
        }
    }
}