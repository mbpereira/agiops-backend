using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Invitations.Common;
using PlanningPoker.Domain.Common.Extensions;

namespace PlanningPoker.Application.Invitations.AcceptInvitation
{
    public class AcceptInvitationCommand : Command
    {
        public int InvitationId { get; private set; }

        public AcceptInvitationCommand(int invitationId)
        {
            SetInvitationId(invitationId);
        }

        private void SetInvitationId(int invitationId)
        {
            if (!invitationId.GreaterThan(0))
            {
                AddError(ChangeInvitationErrors.InvalidInvitationId(nameof(AcceptInvitationCommand)));
                return;
            }

            InvitationId = invitationId;
        }
    }
}