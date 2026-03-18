using System.Net;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ledgerscope.PassiveHealthChecker.Tests;

[TestClass]
public class PassiveHttpHealthCheckStatusTests
{
    [TestMethod]
    public void AddResponse_TracksLatestTimestampForEachStatusCodeSeen()
    {
        var status = new PassiveHttpHealthCheckStatus { Host = "example.com" };

        Assert.IsNull(status.LastSuccess);
        Assert.IsNull(status.LastFailure);
        Assert.AreEqual(0, status.LastSeenByStatusCode.Count);

        status.AddResponse(HttpStatusCode.OK);
        status.AddResponse(HttpStatusCode.MovedPermanently);
        status.AddResponse(HttpStatusCode.BadRequest);
        status.AddResponse(HttpStatusCode.InternalServerError);

        Assert.AreEqual(4, status.LastSeenByStatusCode.Count);
        Assert.AreEqual(status.LastSeenByStatusCode[HttpStatusCode.OK], status.LastSuccess);
        Assert.AreEqual(status.LastSeenByStatusCode[HttpStatusCode.InternalServerError], status.LastFailure);
        Assert.IsTrue(status.LastSeenByStatusCode.ContainsKey(HttpStatusCode.MovedPermanently));
        Assert.IsTrue(status.LastSeenByStatusCode.ContainsKey(HttpStatusCode.BadRequest));
    }
}