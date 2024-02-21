using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Invitations.Shared;
using PlanningPoker.Domain.Common.Extensions;

namespace PlanningPoker.Application.Invitations.RenewInvitation
{
    public class RenewInvitationCommand : Command
    {
        public int Id { get; private set; }

        public RenewInvitationCommand(int id)
        {
            SetInvitationId(id);
        }

        public void SetInvitationId(int invitationId)
        {
            if (!invitationId.GreaterThan(0))
            {
                AddError(ChangeInvitationErrors.InvalidInvitationId(nameof(RenewInvitationCommand)));
                return;
            }

            Id = invitationId;
        }
    }
}