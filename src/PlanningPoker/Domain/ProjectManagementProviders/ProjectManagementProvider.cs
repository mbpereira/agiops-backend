using PlanningPoker.Domain.Abstractions;
using PlanningPoker.Domain.Common.Extensions;

namespace PlanningPoker.Domain.ProjectManagementProviders;

public sealed class ProjectManagementProvider : TenantableAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public Credentials? Credentials { get; private set; }

    private ProjectManagementProvider(string id, string tenantId, string name, Credentials credentials) : base(id,
        tenantId)
    {
        SetName(name);
        Credentials = credentials;
    }

    public void SetName(string name)
    {
        if (!name.HasMinLength(minLength: 3))
        {
            AddError(ProjectManagementProviderErrors.InvalidName);
            return;
        }

        Name = name;
    }


    public static ProjectManagementProvider NewWithApiToken(string tenantId, string name, string apiToken)
    {
        return new ProjectManagementProvider(EntityId.Generate(), tenantId, name, new ApiTokenCredentials(apiToken));
    }
}