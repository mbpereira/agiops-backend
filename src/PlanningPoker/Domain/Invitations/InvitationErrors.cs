#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Domain.Invitations;

public static class InvitationErrors
{
    public static readonly Error InvalidReceiver =
        Error.InvalidEmail(nameof(Invitation), nameof(Invitation.Receiver));

    public static readonly Error ExpiredInvitation =
        new(nameof(Invitation), nameof(Invitation.Accept), "This invitation has expired.");

    public static Error FinishedInvitation(string code)
    {
        return new Error(nameof(Invitation), code,
            "This invitation has already been accepted or is cancelled.");
    }
}