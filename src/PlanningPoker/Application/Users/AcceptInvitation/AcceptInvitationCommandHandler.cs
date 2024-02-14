using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.Application.Users.AcceptInvitation
{
    public class AcceptInvitationCommandHandler : ICommandHandler<AcceptInvitationCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserContext _userContext;

        public AcceptInvitationCommandHandler(IUnitOfWork uow, IUserContext userContext)
        {
            _uow = uow;
            _userContext = userContext;
        }

        public async Task<CommandResult> HandleAsync(AcceptInvitationCommand command)
        {
            if (!command.IsValid)
                return CommandResult.Fail(command.Errors, CommandStatus.ValidationFailed);

            var invitation = await _uow.Invitations.GetByIdAsync(command.InvitationId);

            if (invitation is null)
                return CommandResult.Fail(CommandStatus.RecordNotFound);

            invitation.Accept();

            if (!invitation.IsValid)
                return CommandResult.Fail(invitation.Errors, CommandStatus.ValidationFailed);

            var accessGrants = await GetAccessGrantsAsync(invitation);

            await _uow.Invitations.ChangeAsync(invitation);
            await _uow.AccessGrants.AddAsync(accessGrants);

            await _uow.SaveChangesAsync();

            return CommandResult.Success();
        }

        private async Task<IList<AccessGrant>> GetAccessGrantsAsync(Invitation invitation)
        {
            var userInformation = await _userContext.GetCurrentUserAsync();
            var scopes = TenantScopes.GetByRole(invitation.Role);
            return scopes.Select(scope => AccessGrant.New(userInformation.Id, invitation.TenantId, Resources.Tenant, scope)).ToList();
        }
    }
}
