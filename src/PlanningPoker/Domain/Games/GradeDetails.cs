namespace PlanningPoker.Domain.Games;

public record GradeDetails
{
    internal GradeDetails(IList<string> value)
    {
        Values = value.AsReadOnly();
    }

    public IReadOnlyCollection<string> Values { get; }
    public bool IsQuantifiable => Values.All(item => decimal.TryParse(item, out var _));

    public static GradeDetails Empty()
    {
        return new GradeDetails(new List<string>());
    }
}