using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace CustomTelemetryAndReporting
{
    public static class Processor
    {
        [FunctionName("Processor")]
        [return: Queue("processed")]
        public async static Task<Request> Run([QueueTrigger("to-process")]Request request, TraceWriter log)
        {
            log.Info($"Processor function processed");

            var rand = new Random();
            var randomVal = rand.Next(1, 5);

            await Task.Delay(500 * randomVal);

            TrackRequest(request);

            if(randomVal == 5)
            {
                throw new Exception("Simulate failure 20% of the time");
            }

            return request;
        }

        private static void TrackRequest(Request request)
        {
            TelemetryClient telemetryClient = new TelemetryClient(
                new TelemetryConfiguration
                {
                    InstrumentationKey = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"]
                }
            );

            var trackingEvent = new EventTelemetry("Processed");
            trackingEvent.Context.Operation.Id = request.Id;
            telemetryClient.TrackEvent(trackingEvent);
        }
    }
}
