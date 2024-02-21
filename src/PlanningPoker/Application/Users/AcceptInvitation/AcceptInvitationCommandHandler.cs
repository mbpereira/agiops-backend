using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.Application.Users.AcceptInvitation
{
    public class AcceptInvitationCommandHandler(IUnitOfWork uow, IUserContext userContext)
        : ICommandHandler<AcceptInvitationCommand>
    {
        public async Task<CommandResult> HandleAsync(AcceptInvitationCommand command)
        {
            if (!command.IsValid)
                return CommandResult.Fail(command.Errors, CommandStatus.ValidationFailed);

            var invitation = await uow.Invitations.GetByIdAsync(command.InvitationId);

            if (invitation is null)
                return CommandResult.Fail(CommandStatus.RecordNotFound);

            invitation.Accept();

            if (!invitation.IsValid)
                return CommandResult.Fail(invitation.Errors, CommandStatus.ValidationFailed);

            var accessGrants = await GetAccessGrantsAsync(invitation);

            await uow.Invitations.ChangeAsync(invitation);
            await uow.AccessGrants.AddAsync(accessGrants);

            await uow.SaveChangesAsync();

            return CommandResult.Success();
        }

        private async Task<IList<AccessGrant>> GetAccessGrantsAsync(Invitation invitation)
        {
            var userInformation = await userContext.GetCurrentUserAsync();
            var scopes = TenantScopes.GetByRole(invitation.Role);
            return scopes.Select(scope =>
                AccessGrant.New(userInformation.Id, invitation.TenantId, Resources.Tenant, scope)).ToList();
        }
    }
}