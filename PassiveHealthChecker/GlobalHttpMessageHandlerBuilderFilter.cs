using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace Ledgerscope.PassiveHealthChecker
{
    /// <summary>
    /// Ensure that all HTTP clients use the passive health check handler
    /// </summary>
    public class GlobalHttpMessageHandlerBuilderFilter(IServiceProvider services) : IHttpMessageHandlerBuilderFilter
    {
        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return builder =>
            {
                next(builder);
                var handler = services.GetRequiredService<PassiveHttpHealthCheckHandler>();
                builder.AdditionalHandlers.Insert(0, handler);
            };
        }
    }
}
