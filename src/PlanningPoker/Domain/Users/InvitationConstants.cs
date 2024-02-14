namespace PlanningPoker.Domain.Users
{
    public static class InvitationConstants
    {
        public const int ExpirationTimeInMinutes = 30;

        public static class Messages
        {
            public const string AlreadyAcceptedInvitation = "This invitation has already been accepted or is inactive.";
            public const string ExpiredInvitation = "This invitation has expired.";
        }
    }
}
