namespace PlanningPoker.Domain.Validation
{
    public interface IValidationHandler<TEntity>
    {
        ValidationResult Handle(TEntity instance);
    }
}
