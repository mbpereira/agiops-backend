namespace PlanningPoker.Domain.Common.Extensions;

public static class ValidationExtensions
{
    public static bool IsNull<T>(this T? src)
    {
        return src is null;
    }

    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool IsPresent(this string? str)
    {
        return !string.IsNullOrEmpty(str?.Trim());
    }

    public static bool IsEmail(this string? str)
    {
        return str.IsPresent() && StringPatterns.Email().IsMatch(str!);
    }

    public static bool HasMinLength(this string? str, int minLength)
    {
        return str.IsPresent() && str!.Length >= minLength;
    }

    public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
    {
        return !enumerable.Any();
    }

    public static bool IsSome<T>(this T e, params T[] values) where T : Enum
    {
        return values.Contains(e);
    }
}