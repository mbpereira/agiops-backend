using PlanningPoker.Application.Abstractions.Commands;
using PlanningPoker.Application.Tenants;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Games;

namespace PlanningPoker.Application.Games.CreateIssue
{
    public class CreateIssueCommandHandler(IUnitOfWork uow, ITenantContext tenantContext)
        : ICommandHandler<CreateIssueCommand, CreateIssueResult>
    {
        public async Task<CommandResult<CreateIssueResult>> HandleAsync(CreateIssueCommand command)
        {
            var currentTenant = await tenantContext.GetCurrentTenantAsync();

            var issue = Issue.New(currentTenant.Id, command.GameId, command.Name, command.Description, command.Link);

            if (!issue.IsValid)
                return CommandResult<CreateIssueResult>.Fail(issue.Errors, CommandStatus.ValidationFailed);

            var createdIssue = await uow.Issues.AddAsync(issue);

            await uow.SaveChangesAsync();

            return CommandResult<CreateIssueResult>.Success(new CreateIssueResult(createdIssue.Id.Value));
        }
    }
}