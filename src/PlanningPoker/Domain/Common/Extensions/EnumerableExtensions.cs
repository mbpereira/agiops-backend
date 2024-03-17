namespace PlanningPoker.Domain.Common.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<string> OnlyNotNullOrEmpty(this IEnumerable<string> strings)
    {
        return strings
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s));
    }
}