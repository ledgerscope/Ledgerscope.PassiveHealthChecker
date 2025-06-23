using System.Collections.Concurrent;
using System.Net;

namespace Ledgerscope.PassiveHealthChecker
{
    /// <summary>
    /// Singleton, datastore for HTTP health check statuses
    /// </summary>
    public class PassiveHttpHealthCheckStatuses
    {
        private readonly ConcurrentDictionary<string, PassiveHttpHealthCheckStatus> _statuses = [];

        public void AddResponse(string host, HttpStatusCode response)
        {
            var status = _statuses.GetOrAdd(host, h => new PassiveHttpHealthCheckStatus { Host = h });
            status.AddResponse(response);
        }

        public IEnumerable<PassiveHttpHealthCheckStatus> GetStatuses()
        {
            // Prune old responses before returning statuses
            foreach (var status in _statuses.Values)
            {
                status.PruneResponses();
            }

            return _statuses.Values;
        }
    }
}
