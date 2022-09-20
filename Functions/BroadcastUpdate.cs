using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace adt_signalr_broadcaster.Functions
{
    public class BroadcastUpdate
    {
        private readonly IConfiguration _configuration;
        private readonly string _subjectFilter;

        public BroadcastUpdate(IConfiguration configuration)
        {
            _configuration = configuration;
            _subjectFilter = _configuration["ADT_SUBJECT_FILTER"] ?? "*";
        }

        [FunctionName("BroadcastUpdate")]
        public Task Run([EventHubTrigger("%ADT_EVENTHUB_NAME%", Connection = "ADT_EVENTHUB_CONNECTION_STRING", ConsumerGroup = "%ADT_EVENTHUB_CONSUMERGROUP%")] EventData[] events, [SignalR(HubName = "%SIGNALR_HUB_NAME%", ConnectionStringSetting="SIGNALR_HUB_CONNECTION_STRING")] IAsyncCollector<SignalRMessage> signalRMessages, ILogger log)
        {
            var exceptions = new List<Exception>();

            log.LogInformation($"Messages Received {events.Length}");

            foreach (EventData eventData in events)
            {
                try
                {
                    var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                    // If the message is not a patch ignore it.
                    if(!messageBody.Contains("\"op\"")) {
                        log.LogWarning("Message does not contain an op property, and is not a patch, skipping");
                        continue;
                    }

                    var message = JObject.Parse(messageBody);
                    var subject = eventData.Properties["cloudEvents:subject"].ToString();
                    var patch = message.SelectToken("patch");

                    log.LogDebug($"Subject: {subject}");
                    log.LogDebug($"Patch: {patch}");

                    if(!_subjectFilter.Equals("*") && !subject.Contains(_subjectFilter))
                    {
                        log.LogInformation($"Subject does not match filter: {_subjectFilter}");
                        continue;   
                    }

                    log.LogInformation("Sending message to SignalR Hub for Subject: {subject}", subject);

                    var property = new Dictionary<object, object>
                    {
                        {"TwinId", subject },
                    };

                    var properties = new Dictionary<string, string>();

                    foreach (var item in patch as JArray)
                    {
                        var propertyName = item.SelectToken("path").ToString();
                        var propertyValue = item.SelectToken("value").ToString();
                        properties.Add(propertyName, propertyValue);
                    }

                    property.Add("patch", properties);

                    return signalRMessages.AddAsync(new SignalRMessage
                    {                        
                        Target = subject,
                        Arguments = new[] { property }
                    });
                }
                catch (Exception e)
                {
                    log.LogError(e.ToString());
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();

            return Task.CompletedTask;

        }
    }
}