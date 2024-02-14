using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Application.Users.RenewInvitation
{
    public class RenewInvitationCommandHandler : ICommandHandler<RenewInvitationCommand>
    {
        private readonly IUnitOfWork _uow;

        public RenewInvitationCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CommandResult> HandleAsync(RenewInvitationCommand command)
        {
            if (!command.IsValid)
                return CommandResult.Fail(command.Errors, CommandStatus.ValidationFailed);

            var invitation = await _uow.Invitations.GetByIdAsync(command.Id);

            if (invitation is null)
                return CommandResult.Fail(CommandStatus.RecordNotFound);

            invitation.Renew();

            if (!invitation.IsValid)
                return CommandResult.Fail(invitation.Errors, CommandStatus.ValidationFailed);

            await _uow.Invitations.ChangeAsync(invitation);
            await _uow.SaveChangesAsync();

            return CommandResult.Success();
        }
    }
}
