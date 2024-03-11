#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Abstractions;

#endregion

namespace PlanningPoker.Application.Invitations.RenewInvitation;

public class RenewInvitationCommandHandler(IUnitOfWork uow) : ICommandHandler<RenewInvitationCommand>
{
    public async Task<CommandResult> HandleAsync(RenewInvitationCommand command)
    {
        if (!command.IsValid)
            return (command.Errors, CommandStatus.ValidationFailed);

        var invitation = await uow.Invitations.GetByIdAsync(command.Id);

        if (invitation is null)
            return CommandStatus.RecordNotFound;

        invitation.Renew();

        if (!invitation.IsValid)
            return (invitation.Errors, CommandStatus.ValidationFailed);

        await uow.Invitations.ChangeAsync(invitation);
        await uow.SaveChangesAsync();

        return CommandStatus.Success;
    }
}