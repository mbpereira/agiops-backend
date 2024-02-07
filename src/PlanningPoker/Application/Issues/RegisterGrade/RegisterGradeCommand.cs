using FluentValidation;
using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Issues.RegisterGrade
{
    public class RegisterGradeCommand : Command<RegisterGradeCommand>
    {
        public int IssueId { get; }
        public decimal Grade { get; }

        public RegisterGradeCommand(int issueId, decimal grade)
        {
            IssueId = issueId;
            Grade = grade;
        }

        protected override void ConfigureValidationRules(IValidationRuleFactory<RegisterGradeCommand> validator)
        {
            validator.CreateRuleFor(r => r.IssueId)
                .GreaterThan(0);
        }
    }
}
