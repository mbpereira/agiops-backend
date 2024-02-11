namespace PlanningPoker.Domain.Users
{
    public sealed record Guest
    {
        public string SessionId { get; private set; }

        internal Guest(string sessionId)
        {
            SessionId = sessionId;
        }
    }
}
