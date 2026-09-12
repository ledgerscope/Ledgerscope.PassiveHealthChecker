using System.Collections.Concurrent;
using System.Net;

namespace Ledgerscope.PassiveHealthChecker
{
    /// <summary>
    /// Represents the health status of a single host
    /// </summary>
    public class PassiveHttpHealthCheckStatus
    {
        private readonly ConcurrentDictionary<HttpStatusCode, DateTime> _lastSeenByStatusCode = [];

        public required string Host { get; init; }
        public DateTime? LastSuccess { get; private set; }
        public DateTime? LastFailure { get; private set; }
        public IReadOnlyDictionary<HttpStatusCode, DateTime> LastSeenByStatusCode => _lastSeenByStatusCode;

        public ConcurrentDictionary<DateTime, HttpStatusCode> RequestHistory = [];

        public void AddResponse(HttpStatusCode statusCode)
        {
            var timestamp = DateTime.UtcNow;

            _lastSeenByStatusCode[statusCode] = timestamp;

            switch ((int)statusCode / 100)
            {
                case 2:
                    LastSuccess = timestamp;
                    break;
                case 3: //HttpClient follows redirects automatically so if a 3xx gets logged it's most likely a problem
                    LastFailure = timestamp;
                    break;
                case 4:
                    LastFailure = timestamp;
                    break;
                case 5:
                    LastFailure = timestamp;
                    break;
                default:
                    LastFailure = timestamp;
                    break;
            }

            RequestHistory[timestamp] = statusCode;
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

        /// <summary>
        /// Gets the percentage of successful HTTP responses in the recent request history.
        /// </summary>
        /// <remarks>
        /// If there are no recorded requests in <see cref="RequestHistory"/>, this property returns 100.
        /// This reflects the assumption that, for passive health checks, the absence of requests indicates
        /// no observed failures rather than an unhealthy state.
        /// </remarks>
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
