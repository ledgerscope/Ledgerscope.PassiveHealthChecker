using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ledgerscope.PassiveHealthChecker
{
    /// <summary>
    /// Class to implement the health check for passive HTTP health checks
    /// </summary>
    /// <remarks>
    /// Name could do with improving
    /// </remarks>
    public class PassiveHttpHealthCheckHealthCheck(PassiveHttpHealthCheckStatuses statuses) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var hosts = statuses.GetStatuses().ToList();

            if (hosts.Count == 0)
            {
                return Task.FromResult(HealthCheckResult.Healthy("No HTTP hosts have been checked yet"));
            }

            var hostsData = hosts.ToDictionary(a => a.Host, a => (object)a);

            if (hosts.Any(a => a.SuccessPercentage < 50))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Some HTTP hosts are unhealthy", null, hostsData));
            }
            else if (hosts.Any(a => a.SuccessPercentage < 80))
            {
                return Task.FromResult(HealthCheckResult.Degraded("Some HTTP hosts are unhealthy", null, hostsData));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Healthy("All HTTP hosts are healthy", hostsData));
            }
        }
    }
}
