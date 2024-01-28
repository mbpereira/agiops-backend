using FluentValidation;
using System.Linq.Expressions;

namespace PlanningPoker.Domain.Validation
{
    public interface IValidationRuleFactory<TEntity>
    {
        IRuleBuilderInitial<TEntity, TProperty> CreateFor<TProperty>(Expression<Func<TEntity, TProperty>> expression);
        IRuleBuilderInitial<TEntity, TProperty> CreateFor<TProperty>(Expression<Func<TEntity, TProperty>> expression, string propertyName);
    }
}
