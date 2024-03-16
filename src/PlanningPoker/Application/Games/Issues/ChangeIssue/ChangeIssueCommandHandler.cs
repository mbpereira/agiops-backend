using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Domain.Abstractions;

namespace PlanningPoker.Application.Games.Issues.ChangeIssue;

public class ChangeIssueCommandHandler : ICommandHandler<ChangeIssueCommand, ChangeIssueResult>
{
    private readonly IUnitOfWork _uow;

    public ChangeIssueCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<CommandResult<ChangeIssueResult>> HandleAsync(ChangeIssueCommand command)
    {
        if (!command.IsValid) return (command.Errors, CommandStatus.ValidationFailed);

        var issue = await _uow.Issues.GetByIdAsync(command.Id);

        if (issue is null) return CommandStatus.RecordNotFound;

        var hasAnyChange = command.ApplyChangesTo(issue);

        if (!issue.IsValid) return (issue.Errors, CommandStatus.ValidationFailed);
        
        throw new NotImplementedException();
    }
}