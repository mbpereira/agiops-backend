#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Tenants;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Abstractions.Clock;
using PlanningPoker.Domain.Invitations;

#endregion

namespace PlanningPoker.Application.Invitations.SendInvitation;

public class SendInvitationCommandHandler(
    IUnitOfWork uow,
    ITenantContext tenantContext,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<SendInvitationCommand>
{
    public async Task<CommandResult> HandleAsync(SendInvitationCommand command)
    {
        var tenant = await tenantContext.GetCurrentTenantAsync();

        var invitation = Invitation.New(tenant.Id, command.To, command.Role, dateTimeProvider);

        if (!invitation.IsValid)
            return CommandResult.Fail(invitation.Errors, CommandStatus.ValidationFailed);

        await uow.Invitations.AddAsync(invitation);
        await uow.SaveChangesAsync();

        return CommandResult.Success();
    }
}