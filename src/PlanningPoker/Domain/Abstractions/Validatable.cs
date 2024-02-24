#region

using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Domain.Abstractions;

public abstract class Validatable
{
    private readonly ValidationResult _validationResult = new();
    public bool IsValid => _validationResult.IsValid;
    public IEnumerable<Error> Errors => _validationResult.Errors;

    protected void AddError(Error error)
    {
        _validationResult.AddError(error);
    }
}