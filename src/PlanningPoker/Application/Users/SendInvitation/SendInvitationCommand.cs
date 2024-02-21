using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Users;

namespace PlanningPoker.Application.Users.SendInvitation
{
    public class SendInvitationCommand(string to, Role role) : Command
    {
        public string To { get; private set; } = to;
        public Role Role { get; private set; } = role;
    }
}