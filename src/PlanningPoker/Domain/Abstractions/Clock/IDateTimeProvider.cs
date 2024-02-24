namespace PlanningPoker.Domain.Abstractions.Clock;

public interface IDateTimeProvider
{
    DateTime UtcNow();
}