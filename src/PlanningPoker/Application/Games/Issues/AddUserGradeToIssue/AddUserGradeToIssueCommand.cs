#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Common.Extensions;
using PlanningPoker.Domain.Validation;

#endregion

namespace PlanningPoker.Application.Games.Issues.AddUserGradeToIssue;

public static class RegisterGradeCommandErrors
{
    public static readonly Error InvalidIssueId = Error.GreaterThan(nameof(AddUserGradeToIssueCommand),
        nameof(AddUserGradeToIssueCommand.IssueId));
}

public class AddUserGradeToIssueCommand : Command
{
    public AddUserGradeToIssueCommand(string issueId, string grade)
    {
        SetIssueId(issueId);
        Grade = grade;
    }

    public string IssueId { get; private set; } = string.Empty;
    public string Grade { get; private set; }

    public void SetIssueId(string issueId)
    {
        if (!issueId.IsPresent())
        {
            AddError(RegisterGradeCommandErrors.InvalidIssueId);
            return;
        }

        IssueId = issueId;
    }
}