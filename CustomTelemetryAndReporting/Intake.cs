using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace CustomTelemetryAndReporting
{
    public static class Intake
    {


        [FunctionName("Intake")]
        [return: Queue("to-process")]
        public static async Task<Request> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]string input, TraceWriter log)
        {
            log.Info($"Intake function processed");

            var request = new Request()
            {
                Value = input,
                Id = Guid.NewGuid().ToString()
            };

            TrackRequest(request);

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

            var trackingEvent = new EventTelemetry("Intake");
            trackingEvent.Context.Operation.Id = request.Id;
            telemetryClient.TrackEvent(trackingEvent);
        }
    }
}
