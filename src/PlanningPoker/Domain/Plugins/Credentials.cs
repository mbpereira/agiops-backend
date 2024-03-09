namespace PlanningPoker.Domain.Plugins;

public record Credentials;

public record ApiTokenCredentials(string Token) : Credentials;