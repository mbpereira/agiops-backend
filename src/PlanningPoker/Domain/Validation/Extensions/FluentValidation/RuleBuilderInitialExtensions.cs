using FluentValidation;

namespace PlanningPoker.Domain.Validation.Extensions.FluentValidation
{
    public static class RuleBuilderInitialExtensions
    {
        public static IRuleBuilderInitial<T, TProperty> WithPropertyName<T, TProperty>(this IRuleBuilderInitial<T, TProperty> builder, string propertyName)
        {
            builder.Configure(c => c.PropertyName = propertyName);
            return builder;
        }

        public static IRuleBuilderInitial<T, TProperty> WithPropertyName<T, TProperty>(this IRuleBuilderInitial<T, TProperty> builder, Func<string, string> buildPropertyName)
        {
            builder.Configure(c => c.PropertyName = buildPropertyName(c.PropertyName));
            return builder;
        }
    }
}
