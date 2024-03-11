namespace PlanningPoker.Application.Common.Helpers;

public static class Actions
{
    public static bool ExecuteIfNotNull<T>(T? src, Action<T> fn, Predicate<T>? preCondition = null)
    {
        preCondition ??= _ => true;

        if (src is not null && preCondition(src))
        {
            fn(src);
            return true;
        }

        return false;
    }
}