namespace PlanningPoker.Domain.Validation
{
    public interface IValidator<T> : IValidationHandler<T>, IValidationRuleFactory<T>
    {

    }
}
