using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.Abstractions
{
    public abstract class Validatable<T> where T : Validatable<T>
    {
        private readonly IValidationHandler<T> _validationHandler;

        public Validatable()
        {
            _validationHandler = GetValidator();
        }

        private Validator<T> GetValidator()
        {
            var validator = new Validator<T>();
            ConfigureValidationRules(validator);
            return validator;
        }

        protected abstract void ConfigureValidationRules(IValidationRuleFactory<T> validator);

        public virtual ValidationResult Validate()
            => _validationHandler.Handle((T)this);
    }
}
