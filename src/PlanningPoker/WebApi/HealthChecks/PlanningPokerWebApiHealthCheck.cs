using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApi.HealthChecks;

public class PlanningPokerWebApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
        => Task.FromResult(HealthCheckResult.Healthy("PlanningPokerWebApi is healthy"));
}