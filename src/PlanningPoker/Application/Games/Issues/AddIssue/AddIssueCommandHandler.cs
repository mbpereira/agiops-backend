#region

using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Tenants;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

#endregion

namespace PlanningPoker.Application.Games.Issues.AddIssue;

public class AddIssueCommandHandler(IUnitOfWork uow, ITenantContext tenantContext)
    : ICommandHandler<AddIssueCommand, AddIssueResult>
{
    public async Task<CommandResult<AddIssueResult>> HandleAsync(AddIssueCommand command)
    {
        var currentTenant = await tenantContext.GetCurrentTenantAsync();

        var issue = Issue.New(currentTenant.Id, command.GameId, command.Name, command.Description, command.Link);

        if (!issue.IsValid)
            return CommandResult<AddIssueResult>.Fail(issue.Errors, CommandStatus.ValidationFailed);

        var createdIssue = await uow.Issues.AddAsync(issue);

        await uow.SaveChangesAsync();

        return CommandResult<AddIssueResult>.Success(new AddIssueResult(createdIssue.Id.Value));
    }
}