using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Invitations.Common;

public static class ChangeInvitationErrors
{
    public static Error InvalidInvitationId(string command) =>
        Error.GreaterThan(command, "InvitationId", value: 0);
}