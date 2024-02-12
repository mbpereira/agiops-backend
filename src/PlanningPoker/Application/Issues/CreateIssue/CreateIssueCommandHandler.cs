﻿using PlanningPoker.Application.Abstractions;
using PlanningPoker.Application.Security.Tenant;
using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Issues;

namespace PlanningPoker.Application.Issues.CreateIssue
{
    public class CreateIssueCommandHandler : ICommandHandler<CreateIssueCommand, CreateIssueResult>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITenantContext _tenantContext;

        public CreateIssueCommandHandler(IUnitOfWork uow, ITenantContext tenantContext)
        {
            _uow = uow;
            _tenantContext = tenantContext;
        }

        public async Task<CommandResult<CreateIssueResult>> HandleAsync(CreateIssueCommand command)
        {
            var currentTenant = await _tenantContext.GetCurrentTenantAsync();

            var issue = Issue.New(currentTenant.Id, command.GameId, command.Name, command.Description, command.Link);

            var validationResult = issue.Validate();

            if (!validationResult.Success)
                return CommandResult<CreateIssueResult>.Fail(validationResult.Errors, CommandStatus.ValidationFailed);

            var createdIssue = await _uow.Issues.AddAsync(issue);

            await _uow.SaveChangesAsync();

            return CommandResult<CreateIssueResult>.Success(new CreateIssueResult(createdIssue.Id.Value));
        }
    }
}