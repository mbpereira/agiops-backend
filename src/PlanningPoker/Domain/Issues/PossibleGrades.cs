
namespace PlanningPoker.Domain.Issues
{
    public record PossibleGrades
    {
        public IReadOnlyCollection<string> Value { get; private set; }
        public bool IsQuantifiable => Value.All(item => decimal.TryParse(item, out var _));

        internal PossibleGrades(IList<string> value)
        {
            Value = value.AsReadOnly();
        }

        public static PossibleGrades Empty() => new(new List<string>());
    }
}
