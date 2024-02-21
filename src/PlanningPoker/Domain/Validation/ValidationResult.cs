using System.Collections.Immutable;

namespace PlanningPoker.Domain.Validation
{
    public interface IValidationResult
    {
        IImmutableSet<Error> Errors { get; }
        bool IsValid { get; }
    }

    public record ValidationResult : IValidationResult
    {
        private readonly ISet<Error> _errors;
        public IImmutableSet<Error> Errors => _errors.ToImmutableHashSet();

        public bool IsValid => Errors.Count == 0;

        public ValidationResult()
        {
            _errors = new HashSet<Error>();
        }

        public ValidationResult(ISet<Error> errors)
        {
            _errors = errors;
        }

        public void AddError(string code, string message)
        {
            _errors.Add(new Error(code, message));
        }

        public void Merge(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
                AddError(error.Code, error.Message);
        }

        public void AddError(Error error)
        {
            _errors.Add(error);
        }
    }
}