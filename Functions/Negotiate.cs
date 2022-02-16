

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace adt_signalr_broadcaster.Functions
{
    public class Negotiate
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, [SignalRConnectionInfo(HubName = "%SIGNALR_HUB_NAME%")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}