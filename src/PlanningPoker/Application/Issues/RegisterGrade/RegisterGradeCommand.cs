using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Application.Issues.RegisterGrade
{
    public static class RegisterGradeCommandErrors
    {
        public static readonly Error InvalidIssueId = Error.GreaterThan(nameof(RegisterGradeCommand),
            nameof(RegisterGradeCommand.IssueId), value: 0);
    }

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
                AddError(RegisterGradeCommandErrors.InvalidIssueId);
                return;
            }

            IssueId = issueId;
        }
    }
}