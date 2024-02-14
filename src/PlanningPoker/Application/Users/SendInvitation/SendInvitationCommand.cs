using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.Application.Users.SendInvitation
{
    public class SendInvitationCommand : Command
    {
        public string To { get; private set; }
        public Role Role { get; private set; }

        public SendInvitationCommand(string to, Role role)
        {
            To = to;
            Role = role;
        }
    }
}