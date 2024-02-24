#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Tenants;
using PlanningPoker.Domain.Users;

#endregion

namespace PlanningPoker.Application.Tenants.CreateTenant;

public class CreateTenantCommandHandler(IUnitOfWork uow, IUserContext userContext)
    : ICommandHandler<CreateTenantCommand, CreateTenantResult>
{
    public async Task<CommandResult<CreateTenantResult>> HandleAsync(CreateTenantCommand command)
    {
        var tenant = Tenant.New(command.Name);

        if (!tenant.IsValid)
            return CommandResult<CreateTenantResult>.Fail(tenant.Errors, CommandStatus.ValidationFailed);

        var tenantId = await CreateTenantAsync(tenant);

        await CreateAccessGrantsAsync(tenantId);

        return CommandResult<CreateTenantResult>.Success(new CreateTenantResult(tenantId));
    }

    private async Task<string> GetCurrentUserIdAsync()
    {
        var user = await userContext.GetCurrentUserAsync();
        return user.Id;
    }

    private async Task<string> CreateTenantAsync(Tenant tenant)
    {
        var createdTenant = await uow.Tenants.AddAsync(tenant);

        await uow.SaveChangesAsync();

        return createdTenant.Id.Value;
    }

    private async Task CreateAccessGrantsAsync(string tenantId)
    {
        var accessGrants = await GetAccessGrantsAsync(tenantId);

        await uow.AccessGrants.AddAsync(accessGrants);

        await uow.SaveChangesAsync();
    }

    private async Task<IList<AccessGrant>> GetAccessGrantsAsync(string tenantId)
    {
        var userId = await GetCurrentUserIdAsync();

        return TenantScopes
            .GetByRole(Role.Admin)
            .Select(scope => AccessGrant.New(userId, tenantId, Resources.Tenant, scope))
            .ToList();
    }
}