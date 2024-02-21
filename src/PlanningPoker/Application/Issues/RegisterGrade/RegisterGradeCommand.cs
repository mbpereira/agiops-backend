using PlanningPoker.Application.Abstractions;
using PlanningPoker.Domain.Shared.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Issues.RegisterGrade
{
    public class RegisterGradeCommand : Command
    {
        public int IssueId { get; private set; }
        public string Grade { get; private set; }

        public RegisterGradeCommand(int issueId, string grade)
        {
            SetIssueId(issueId);
            Grade = grade;
        }

        public void SetIssueId(int issueId)
        {
            if (!issueId.GreaterThan(0))
            {
                AddError(Error.GreaterThan(nameof(RegisterGradeCommand), nameof(issueId), value: 0));
                return;
            }

            IssueId = issueId;
        }
    }
}