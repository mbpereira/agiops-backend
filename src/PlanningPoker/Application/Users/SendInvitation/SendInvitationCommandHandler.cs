using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.Application.Users.SendInvitation
{
    public class SendInvitationCommandHandler : ICommandHandler<SendInvitationCommand>
    {
        private readonly ITenantContext _tenantContext;
        private readonly IUnitOfWork _uow;

        public SendInvitationCommandHandler(IUnitOfWork uow, ITenantContext tenantContext)
        {
            _uow = uow;
            _tenantContext = tenantContext;
        }

        public async Task<CommandResult> HandleAsync(SendInvitationCommand command)
        {
            var tenant = await _tenantContext.GetCurrentTenantAsync();

            var invitation = Invitation.New(tenant.Id, command.To, command.Role);

            if (!invitation.IsValid)
                return CommandResult.Fail(invitation.Errors, CommandStatus.ValidationFailed);

            await _uow.Invitations.AddAsync(invitation);
            await _uow.SaveChangesAsync();

            return CommandResult.Success();
        }
    }
}
