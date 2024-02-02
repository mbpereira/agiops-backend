namespace PlanningPoker.Domain.Validation
{
    public record ValidationResult
    {
        public IReadOnlyCollection<Error> Errors { get; } = new List<Error>();

        public bool Success => Errors.Count == 0;

        public ValidationResult(IEnumerable<Error> errors)
        {
            Errors = errors.ToList();
        }
    }
}
