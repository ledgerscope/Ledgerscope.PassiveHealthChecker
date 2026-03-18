using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http;

namespace Ledgerscope.PassiveHealthChecker
{
    /// <summary>
    /// Extensions for registering the passive health checker services.
    /// </summary>
    public static class PassiveHealthCheckerServiceCollectionExtensions
    {
        private const string DefaultHealthCheckName = "Passive Http Health Check";

        /// <summary>
        /// Registers the passive health checker services, global HTTP handler filter, and health check.
        /// </summary>
        public static T ConfigurePassiveHealthChecker<T>(this T services) where T : IServiceCollection
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddHttpClient();

            services.TryAddSingleton<PassiveHttpHealthCheckStatuses>();
            services.TryAddTransient<PassiveHttpHealthCheckHandler>();
            services.TryAddTransient<PassiveHttpHealthCheckHealthCheck>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IHttpMessageHandlerBuilderFilter, GlobalHttpMessageHandlerBuilderFilter>());

            services.AddHealthChecks();
            services.Configure<HealthCheckServiceOptions>(options =>
            {
                if (options.Registrations.Any(registration => string.Equals(registration.Name, DefaultHealthCheckName, StringComparison.Ordinal)))
                {
                    return;
                }

                options.Registrations.Add(new HealthCheckRegistration(
                    DefaultHealthCheckName,
                    serviceProvider => serviceProvider.GetRequiredService<PassiveHttpHealthCheckHealthCheck>(),
                    failureStatus: null,
                    tags: null,
                    timeout: default));
            });

            return services;
        }
    }
}