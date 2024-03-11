#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Users;
using PlanningPoker.Domain.Abstractions;

#endregion

namespace PlanningPoker.Application.Games.Issues.AddUserGradeToIssue;

public class AddUserGradeToIssueCommandHandler(IUnitOfWork uow, IUserContext authenticationContext)
    : ICommandHandler<AddUserGradeToIssueCommand>
{
    public async Task<CommandResult> HandleAsync(AddUserGradeToIssueCommand toIssueCommand)
    {
        if (!toIssueCommand.IsValid)
            return (toIssueCommand.Errors, CommandStatus.ValidationFailed);

        var issue = await uow.Issues.GetByIdAsync(toIssueCommand.IssueId);

        if (issue is null)
            return CommandStatus.RecordNotFound;

        var userInformation = await authenticationContext.GetCurrentUserAsync();

        issue.RegisterGrade(userInformation.Id, toIssueCommand.Grade);

        if (!issue.IsValid)
            return (issue.Errors, CommandStatus.ValidationFailed);

        await uow.Issues.ChangeAsync(issue);
        await uow.SaveChangesAsync();

        return CommandStatus.Success;
    }
}