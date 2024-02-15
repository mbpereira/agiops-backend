namespace PlanningPoker.Domain.Shared.Extensions
{
    public static class ValidationExtensions
    {
        public static bool GreaterThan(this int a, int value) => a > value;
        public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);
        public static bool IsPresent(this string? str) => !string.IsNullOrEmpty(str?.Trim());
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

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();
        public static bool IsSome<T>(this T e, params T[] values) where T : Enum => values.Contains(e);
    }
}
