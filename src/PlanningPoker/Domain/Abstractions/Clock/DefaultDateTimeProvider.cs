namespace PlanningPoker.Domain.Abstractions.Clock;

public class DefaultDateTimeProvider : IDateTimeProvider
{
    public static readonly DefaultDateTimeProvider Instance = new();

    private DefaultDateTimeProvider()
    {
    }

    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }
}