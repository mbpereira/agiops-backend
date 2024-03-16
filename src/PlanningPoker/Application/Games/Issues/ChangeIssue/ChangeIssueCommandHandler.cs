#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Abstractions;

#endregion

namespace PlanningPoker.Application.Games.Issues.ChangeIssue;

public class ChangeIssueCommandHandler(IUnitOfWork uow) : ICommandHandler<ChangeIssueCommand, ChangeIssueResult>
{
    public async Task<CommandResult<ChangeIssueResult>> HandleAsync(ChangeIssueCommand command)
    {
        if (!command.IsValid) return (command.Errors, CommandStatus.ValidationFailed);

        var issue = await uow.Issues.GetByIdAsync(command.Id);

        if (issue is null) return CommandStatus.RecordNotFound;

        var hasAnyChange = command.ApplyChangesTo(issue);

        if (!hasAnyChange) return new ChangeIssueResult(issue);

        if (!issue.IsValid) return (issue.Errors, CommandStatus.ValidationFailed);

        await uow.Issues.ChangeAsync(issue);

        return new ChangeIssueResult(issue);
    }
}