using PlanningPoker.Domain.Validation;

namespace PlanningPoker.Domain.ProjectManagementProviders;

public static class ProjectManagementProviderErrors
{
    public static readonly Error InvalidName = Error.MinLength(nameof(ProjectManagementProvider),
        nameof(ProjectManagementProvider.Name), minLength: 3);
}