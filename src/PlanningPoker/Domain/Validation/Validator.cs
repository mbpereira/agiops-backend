using FluentValidation;
using PlanningPoker.Domain.Validation.Extensions.FluentValidation;
using System.Linq.Expressions;

namespace PlanningPoker.Domain.Validation
{
    public class Validator<T> : AbstractValidator<T>, IValidator<T>
    {
        private readonly string _className = typeof(T).Name;

        private string BuildPropertyName(string originalPropertyName) => $"{_className}.{originalPropertyName}";

        public IRuleBuilderInitial<T, TProperty> CreateRuleFor<TProperty>(Expression<Func<T, TProperty>> expression)
            => RuleFor(expression)
                .WithPropertyName(BuildPropertyName);

        public IRuleBuilderInitial<T, TProperty> CreateRuleFor<TProperty>(Expression<Func<T, TProperty>> expression, string propertyName)
            => RuleFor(expression)
                .WithPropertyName(original => BuildPropertyName(propertyName));

        public ValidationResult Handle(T instance)
        {
            var result = Validate(instance);
            var errors = result
                .Errors
                .GroupBy(e => e.PropertyName)
                .Select(e => new Error(e.Key, string.Join("; ", e.Select(g => g.ErrorMessage))))
                .ToList();
            return new ValidationResult(errors);
        }
    }
}
