using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

//using Yajat.Digitalizers.ExpressionProcessor.Service;

using Yajat.Digitalizers.AspStd.ServiceFabric.Common;
using Yajat.Digitalizers.AspStd.ServiceFabric.Logging.Serilog;
using Serilog;
using Serilog.ExtensionMethods;

namespace ExpProcessor
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("ExpProcessorType",
                    //context => new ExpProcessor(context)
                    (context) =>
                    {
                        //ConfigurationPackage configPackage = context.CodePackageActivationContext.GetConfigurationPackageObject(@"Config");
                        ILogger serilog = new LoggerConfiguration()
                            .WriteTo.ApplicationInsights("6c414b15-074d-4fa5-af61-131412afe2a1", ConvertLogEventsToCustomTraceTelemetry)
                             .CreateLogger();
                        Log.Logger = serilog;
                        return new ExpProcessor(context, serilog.Enrich<ExpProcessor>(context));
                        //return new ExpProcessor(context);
                    }
                    ).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(ExpProcessor).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        public static Microsoft.ApplicationInsights.Channel.ITelemetry ConvertLogEventsToCustomTraceTelemetry(Serilog.Events.LogEvent logEvent, IFormatProvider formatProvider)
        {
            // first create a default TraceTelemetry using the sink's default logic
            // .. but without the log level, and (rendered) message (template) included in the Properties
            var telemetry = logEvent.ToDefaultTraceTelemetry(
                formatProvider,
                includeLogLevelAsProperty: false,
                includeRenderedMessageAsProperty: false,
                includeMessageTemplateAsProperty: false);

            // then go ahead and post-process the telemetry's context to contain the user id as desired
            if (logEvent.Properties.ContainsKey("UserId"))
            {
                telemetry.Context.User.Id = logEvent.Properties["UserId"].ToString();
            }

            // and remove UserId and HttpRequestId from the telemetry properties
            if (telemetry.Properties.ContainsKey("UserId"))
            {
                telemetry.Properties.Remove("UserId");
            }

            if (telemetry.Properties.ContainsKey("HttpRequestId"))
            {
                telemetry.Properties.Remove("HttpRequestId");
            }

            return telemetry;
        }
    }
}
