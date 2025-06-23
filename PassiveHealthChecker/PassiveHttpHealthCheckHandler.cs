using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledgerscope.PassiveHealthChecker
{
    /// <summary>
    /// Can hook into the HTTP pipeline to monitor responses to check for errors
    /// </summary>
    public class PassiveHttpHealthCheckHandler(PassiveHttpHealthCheckStatuses statuses) : DelegatingHandler
    {
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var response = base.Send(request, cancellationToken);

                statuses.AddResponse(request.RequestUri.Host, response.StatusCode);

                return response;
            }
            catch (HttpRequestException ex)
            {
                statuses.AddResponse(request.RequestUri.Host, ex.StatusCode.GetValueOrDefault());
                throw;
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                statuses.AddResponse(request.RequestUri.Host, response.StatusCode);

                return response;
            }
            catch (HttpRequestException ex)
            {
                statuses.AddResponse(request.RequestUri.Host, ex.StatusCode.GetValueOrDefault());
                throw;
            }
        }
    }
}
