using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Abstractions
{
    public abstract class Validatable<T> where T : Validatable<T>
    {
        private readonly IValidationHandler<T> _validationHandler;
        private readonly ValidationResult _validationResult;

        public Validatable()
        {
            _validationResult = new ValidationResult();
            _validationHandler = GetValidator();
        }

        private Validator<T> GetValidator()
        {
            var validator = new Validator<T>();
            ConfigureValidationRules(validator);
            return validator;
        }

        protected void AddError(string code, string message)
        {
            var className = typeof(T).Name;
            _validationResult.AddError($"{className}.{code}", message);
        }

        protected abstract void ConfigureValidationRules(IValidationRuleFactory<T> validator);

        public virtual IValidationResult Validate()
        {
            _validationResult.Merge(_validationHandler.Handle((T)this));
            return _validationResult;
        }
    }
}
