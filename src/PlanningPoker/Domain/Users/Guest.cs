namespace PlanningPoker.Domain.Users;

public sealed record Guest
{
    internal Guest(string sessionId)
    {
        SessionId = sessionId;
    }

    public string SessionId { get; private set; }

    public static Guest New()
    {
        return new Guest(Guid.NewGuid().ToString());
    }
}