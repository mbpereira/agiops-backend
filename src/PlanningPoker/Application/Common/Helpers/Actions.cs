namespace PlanningPoker.Application.Common.Helpers;

public static class Actions
{
    public static bool ExecuteIfNotNull<T>(T? src, Action<T> fn, Predicate<T>? preCondition = null)
    {
        preCondition ??= _ => true;

        var canExecute = src is not null && preCondition(src);

        if (!canExecute) return false;

        fn(src!);

        return true;
    }
}