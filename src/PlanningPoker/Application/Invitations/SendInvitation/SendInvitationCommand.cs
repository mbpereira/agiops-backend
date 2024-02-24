#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Users;

#endregion

namespace PlanningPoker.Application.Invitations.SendInvitation;

public class SendInvitationCommand(string to, Role role) : Command
{
    public string To { get; private set; } = to;
    public Role Role { get; private set; } = role;
}