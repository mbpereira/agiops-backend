using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Abstractions
{
    public abstract class Validatable
    {
        private readonly ValidationResult _validationResult;
        public bool IsValid => _validationResult.IsValid;
        public IEnumerable<Error> Errors => _validationResult.Errors;

        public Validatable()
        {
            _validationResult = new ValidationResult();
        }

        protected void AddError(Error error)
        {
            _validationResult.AddError(error);
        }
    }
}
