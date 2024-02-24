#region

using Microsoft.Extensions.Diagnostics.HealthChecks;

#endregion

namespace PlanningPoker.WebApi.HealthChecks;

public class PlanningPokerWebApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        return Task.FromResult(HealthCheckResult.Healthy("PlanningPokerWebApi is healthy"));
    }
}