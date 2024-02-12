using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.Application.Users.CreateTenant
{
    public class CreateTenantCommandHandler : ICommandHandler<CreateTenantCommand, CreateTenantResult>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserContext _userContext;

        public CreateTenantCommandHandler(IUnitOfWork uow, IUserContext userContext)
        {
            _uow = uow;
            _userContext = userContext;
        }

        public async Task<CommandResult<CreateTenantResult>> HandleAsync(CreateTenantCommand command)
        {
            var tenant = Tenant.New(command.Name);

            var validationResult = tenant.Validate();

            if (!validationResult.Success)
                return CommandResult<CreateTenantResult>.Fail(validationResult.Errors, CommandStatus.ValidationFailed);

            var tenantId = await CreateTenantAsync(tenant);

            var userId = await GetCurrentUserIdAsync();

            await CreateAccessGrantsAsync(tenantId, userId);

            return CommandResult<CreateTenantResult>.Success(new CreateTenantResult(tenantId));
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var user = await _userContext.GetCurrentUserAsync();
            return user.Id;
        }

        private async Task<int> CreateTenantAsync(Tenant tenant)
        {
            var createdTenant = await _uow.Tenants.AddAsync(tenant);

            await _uow.SaveChangesAsync();

            return createdTenant.Id.Value;
        }

        private async Task CreateAccessGrantsAsync(int tenantId, int userId)
        {
            var accessGrants = TenantScopes
                .GetByRole(Role.Admin)
                .Select(scope => AccessGrant.New(userId, tenantId, Resources.Tenant, scope))
                .ToList();

            await _uow.AccessGrants.AddAsync(accessGrants);

            await _uow.SaveChangesAsync();
        }
    }
}
