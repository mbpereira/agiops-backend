using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public static class InvitationErrors
    {
        public static Error InvalidReceiver = Error.InvalidEmail(nameof(Invitation), nameof(Invitation.Receiver));
        public static Error AlreadyAcceptedInvitation(string code) => new(nameof(Invitation), code, "This invitation has already been accepted or is inactive.");
        public static Error ExpiredInvitation = new Error(nameof(Invitation), nameof(Invitation.Accept), "This invitation has expired.");
    }
}
