#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Invitations.Common;
using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Application.Invitations.AcceptInvitation;

public class AcceptInvitationCommand : Command
{
    public AcceptInvitationCommand(string invitationId)
    {
        SetInvitationId(invitationId);
    }

    public string InvitationId { get; private set; } = string.Empty;

    private void SetInvitationId(string invitationId)
    {
        if (!invitationId.IsPresent())
        {
            AddError(ChangeInvitationErrors.InvalidInvitationId(nameof(AcceptInvitationCommand)));
            return;
        }

        InvitationId = invitationId;
    }
}