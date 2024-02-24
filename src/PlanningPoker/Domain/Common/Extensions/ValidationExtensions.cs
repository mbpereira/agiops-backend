namespace PlanningPoker.Domain.Common.Extensions;

public static class ValidationExtensions
{
    public static bool GreaterThan(this int a, int value)
    {
        return a > value;
    }

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
        if (str.IsNullOrEmpty()) return false;
        return StringPatterns.Email().IsMatch(str!);
    }

    public static bool HasMinLength(this string? str, int minLength)
    {
        if (str.IsNullOrEmpty()) return false;
        return str!.Length >= minLength;
    }

    public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
    {
        return !enumerable.Any();
    }

    public static bool IsSome<T>(this T e, params T[] values) where T : Enum
    {
        return values.Contains(e);
    }

    public static IEnumerable<string> OnlyNotNullOrEmpty(this IEnumerable<string> strings)
    {
        return strings
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s));
    }
}