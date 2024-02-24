#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Application.Invitations.Common;

public static class ChangeInvitationErrors
{
    public static Error InvalidInvitationId(string command)
    {
        return Error.GreaterThan(command, "InvitationId");
    }
}