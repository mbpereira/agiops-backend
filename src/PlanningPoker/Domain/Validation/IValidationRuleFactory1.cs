using FluentValidation;
using System.Linq.Expressions;

namespace PlanningPoker.Domain.Validation
{
    public interface IValidationRuleFactory1<TEntity>
    {
        IRuleBuilderInitial<TEntity, TProperty> CreateFor<TProperty>(Expression<Func<TEntity, TProperty>> expression);
    }
}