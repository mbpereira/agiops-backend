using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Users
{
    public static class InvitationErrors
    {
        public static readonly Error InvalidReceiver = Error.InvalidEmail(nameof(Invitation), nameof(Invitation.Receiver));
        public static readonly Error ExpiredInvitation = new Error(nameof(Invitation), nameof(Invitation.Accept), "This invitation has expired.");
        public static Error AlreadyAcceptedInvitation(string code) => new(nameof(Invitation), code, "This invitation has already been accepted or is inactive.");
    }
}
