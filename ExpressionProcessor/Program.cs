using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using Yajat.Digitalizers.ExpressionProcessor.Service;

using Yajat.Digitalizers.AspStd.ServiceFabric.Common;
using Yajat.Digitalizers.AspStd.ServiceFabric.Logging.Serilog;
using Serilog;
using Serilog.ExtensionMethods;

namespace Yajat.Digitalizers.ExpressionProcessor.Service
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

                ServiceRuntime.RegisterServiceAsync("ExpressionProcessorType",
                    //context => new ExpressionProcessor(context)
                    (context) =>
                    {
                        ConfigurationPackage configPackage = context.CodePackageActivationContext.GetConfigurationPackageObject(@"Config");
                        ILogger serilog = new LoggerConfiguration()
                            .WriteTo.ApplicationInsights(configPackage.Settings.Sections[@"ResourceSettings"].Parameters[@"ApplicationInsights_Key"].Value, Hosting.ConvertLogEventsToCustomTraceTelemetry)
                             .CreateLogger();
                        Log.Logger = serilog;
                        return new ExpressionProcessor(context, serilog.Enrich<ExpressionProcessor>(context));
                    }
                    ).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(ExpressionProcessor).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
}
}
