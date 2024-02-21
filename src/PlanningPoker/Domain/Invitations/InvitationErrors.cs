using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Invitations
{
    public static class InvitationErrors
    {
        public static readonly Error InvalidReceiver =
            Error.InvalidEmail(nameof(Invitation), nameof(Invitation.Receiver));

        public static readonly Error ExpiredInvitation =
            new(nameof(Invitation), nameof(Invitation.Accept), "This invitation has expired.");

        public static Error AlreadyAcceptedInvitation(string code) => new(nameof(Invitation), code,
            "This invitation has already been accepted or is inactive.");
    }
}