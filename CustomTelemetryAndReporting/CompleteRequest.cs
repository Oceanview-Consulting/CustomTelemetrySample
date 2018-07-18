using System;
using System.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace CustomTelemetryAndReporting
{
    public static class CompleteRequest
    {
        [FunctionName("CompleteRequest")]
        public static void Run([QueueTrigger("processed")]Request request, TraceWriter log)
        {
            log.Info($"CompleteRequest function processed");

            TrackRequest(request);
        }

        private static void TrackRequest(Request request)
        {
            TelemetryClient telemetryClient = new TelemetryClient(
                new TelemetryConfiguration
                {
                    InstrumentationKey = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"]
                }
            );

            var trackingEvent = new EventTelemetry("Complete");
            trackingEvent.Context.Operation.Id = request.Id;
            telemetryClient.TrackEvent(trackingEvent);
        }
    }
}
