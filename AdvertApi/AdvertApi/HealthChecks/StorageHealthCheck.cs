using AdvertApi.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace AdvertApi.HealthChecks
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly IAdvertStorageService advertStorageService;

        public StorageHealthCheck(IAdvertStorageService advertStorageService)
        {
            this.advertStorageService = advertStorageService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isAlive = await advertStorageService.CheckHealthAsync();

            return isAlive ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
    }
}
