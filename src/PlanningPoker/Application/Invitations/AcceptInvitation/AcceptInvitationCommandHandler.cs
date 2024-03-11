#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Invitations;
using PlanningPoker.Domain.Tenants;
using PlanningPoker.Domain.Users;

#endregion

namespace PlanningPoker.Application.Invitations.AcceptInvitation;

public class AcceptInvitationCommandHandler(IUnitOfWork uow, IUserContext userContext)
    : ICommandHandler<AcceptInvitationCommand>
{
    public async Task<CommandResult> HandleAsync(AcceptInvitationCommand command)
    {
        if (!command.IsValid)
            return (command.Errors, CommandStatus.ValidationFailed);

        var invitation = await uow.Invitations.GetByIdAsync(command.InvitationId);

        if (invitation is null)
            return CommandStatus.RecordNotFound;

        invitation.Accept();

        if (!invitation.IsValid)
            return (invitation.Errors, CommandStatus.ValidationFailed);

        var accessGrants = await GetAccessGrantsAsync(invitation);

        await uow.Invitations.ChangeAsync(invitation);
        await uow.AccessGrants.AddAsync(accessGrants);

        await uow.SaveChangesAsync();

        return CommandStatus.Success;
    }

    private async Task<IList<AccessGrant>> GetAccessGrantsAsync(Invitation invitation)
    {
        var userInformation = await userContext.GetCurrentUserAsync();
        var scopes = TenantScopes.GetByRole(invitation.Role);
        return scopes.Select(scope =>
            AccessGrant.New(userInformation.Id, invitation.TenantId, Resources.Tenant, scope)).ToList();
    }
}