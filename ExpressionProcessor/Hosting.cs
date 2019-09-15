using Yajat.Digitalizers.AspStd.ServiceFabric.Common;
using Yajat.Digitalizers.AspStd.ServiceFabric.Logging.Serilog;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;
using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Serilog.ExtensionMethods;

namespace Yajat.Digitalizers.ExpressionProcessor.Service
{
    internal static class Hosting
    {
        public static void Register()
        {
            ServiceRuntime.RegisterServiceAsync(
                Naming.ServiceType<ExpressionProcessor>(),
                (context) =>
                {
                    ConfigurationPackage configPackage = context.CodePackageActivationContext.GetConfigurationPackageObject(@"Config");
                    ILogger serilog = new LoggerConfiguration()
                        .WriteTo.ApplicationInsights(configPackage.Settings.Sections[@"ResourceSettings"].Parameters[@"ApplicationInsights_Key"].Value, ConvertLogEventsToCustomTraceTelemetry)
                         .CreateLogger();
                    Log.Logger = serilog;
                    return new ExpressionProcessor(context, serilog.Enrich<ExpressionProcessor>(context));
                })
                .GetAwaiter().GetResult();

            ServiceEventSource.Current.ServiceTypeRegistered(
                Process.GetCurrentProcess().Id,
                Naming.ServiceType<ExpressionProcessor>());
        }

        //private static void Main()
        //{
        //    try
        //    {
        //        Register();
        //        Thread.Sleep(Timeout.Infinite);
        //    }
        //    catch (Exception e)
        //    {
        //        ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
        //        throw;
        //    }
        //}

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
