
namespace PlanningPoker.Domain.Issues
{
    public record GradeDetails
    {
        public IReadOnlyCollection<string> Values { get; private set; }
        public bool IsQuantifiable => Values.All(item => decimal.TryParse(item, out var _));

        internal GradeDetails(IList<string> value)
        {
            Values = value.AsReadOnly();
        }

        public static GradeDetails Empty() => new(new List<string>());
    }
}
