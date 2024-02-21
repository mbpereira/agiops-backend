using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Users.RenewInvitation
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
                AddError(Error.GreaterThan(nameof(RenewInvitationCommand), nameof(invitationId), value: 0));
                return;
            }

            Id = invitationId;
        }
    }
}