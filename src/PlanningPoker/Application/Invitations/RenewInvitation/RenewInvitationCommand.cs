#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Invitations.Common;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Application.Invitations.RenewInvitation;

public class RenewInvitationCommand : Command
{
    public RenewInvitationCommand(string id)
    {
        SetInvitationId(id);
    }

    public string Id { get; private set; } = string.Empty;

    public void SetInvitationId(string invitationId)
    {
        if (!invitationId.IsPresent())
        {
            AddError(ChangeInvitationErrors.InvalidInvitationId(nameof(RenewInvitationCommand)));
            return;
        }

        Id = invitationId;
    }
}