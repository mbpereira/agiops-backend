namespace PlanningPoker.Domain.ProjectManagementProviders;

public record Credentials;

public record ApiTokenCredentials(string Token) : Credentials;