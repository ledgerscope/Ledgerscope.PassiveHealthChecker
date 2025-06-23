using System.Collections.Concurrent;
using System.Net;

namespace Ledgerscope.PassiveHealthChecker
{
    /// <summary>
    /// Represents the health status of a single host
    /// </summary>
    public class PassiveHttpHealthCheckStatus
    {
        public required string Host { get; init; }
        public DateTime LastSuccess { get; private set; }
        public DateTime LastFailure { get; private set; }

        public ConcurrentDictionary<DateTime, HttpStatusCode> RequestHistory = [];

        public void AddResponse(HttpStatusCode statusCode)
        {
            if (statusCode >= HttpStatusCode.OK && statusCode <= (HttpStatusCode)299)
            {
                LastSuccess = DateTime.UtcNow;
            }
            else
            {
                LastFailure = DateTime.UtcNow;
            }

            RequestHistory[DateTime.UtcNow] = statusCode;
        }

        private const int MinHistorySize = 100; // Make sure we've always got some history

        public void PruneResponses()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-5); // Keep only the last 5 minutes of responses
            foreach (var key in RequestHistory.Keys.Where(k => k < cutoff).OrderBy(a => a).ToList())
            {
                if (RequestHistory.Count <= MinHistorySize)
                    break; // Stop pruning if we have enough history

                RequestHistory.TryRemove(key, out _);
            }
        }

        public int SuccessPercentage
        {
            get
            {
                if (RequestHistory.IsEmpty)
                    return 100; // No requests, assume healthy

                var successCount = RequestHistory.Count(kv => kv.Value >= HttpStatusCode.OK && kv.Value <= (HttpStatusCode)299);
                return (int)((double)successCount / RequestHistory.Count * 100);
            }
        }
    }
}
