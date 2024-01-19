namespace Domain.Validation
{
    public record ValidationResult
    {
        public IReadOnlyCollection<Error> Errors { get; } = new List<Error>();

        public bool IsValid => Errors.Count == 0;

        public ValidationResult(FluentValidation.Results.ValidationResult result)
        {
            Errors = result
                .Errors
                .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
                .ToList();
        }

        public ValidationResult(IEnumerable<Error> errors)
        {
            Errors = errors.ToList();
        }
    }
}
