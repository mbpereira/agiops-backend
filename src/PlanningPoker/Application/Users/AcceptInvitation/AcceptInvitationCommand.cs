using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Users.AcceptInvitation
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
                AddError(Error.GreaterThan(nameof(AcceptInvitationCommand), nameof(invitationId), value: 0));
                return;
            }

            InvitationId = invitationId;
        }
    }
}