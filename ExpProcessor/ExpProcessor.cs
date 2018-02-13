using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Yajat.Digitalizers.ExpressionProcessor.Contracts;
using Yajat.Digitalizers.ExpressionProcessor.Impl;

namespace ExpProcessor
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class ExpProcessor : StatelessService, IExpressionProcessor
    {
        private readonly IExpressionProcessor _Impl;
        private readonly ILogger<IExpressionProcessor> _Logger;

        public ExpProcessor(StatelessServiceContext context, ILogger<IExpressionProcessor> logger)
            : base(context)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _Impl = new ExpressionProcessor(logger);
            _Logger.LogInformation("Constructed");
        }

        public Task<string> EvaluateExpressionAsync(string expression)
        {
            return _Impl.EvaluateExpressionAsync(expression);
        }

        public Task<List<string>> ParseExpressionAsync(string expression)
        {
            return _Impl.ParseExpressionAsync(expression);
        }

        protected override Task OnCloseAsync(CancellationToken cancellationToken)
        {
            _Logger.LogInformation($"{nameof(OnCloseAsync)} invoked");
            return base.OnCloseAsync(cancellationToken);
        }

        protected override void OnAbort()
        {
            _Logger.LogInformation($"{nameof(OnAbort)} invoked");
            base.OnAbort();
        }
        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            //return new ServiceInstanceListener[0];
            return new[]
           {
                new ServiceInstanceListener(
                    (context) => new FabricTransportServiceRemotingListener(
                        context,
                        this,
                        new FabricTransportRemotingListenerSettings
                        {
                            EndpointResourceName = typeof(IExpressionProcessor).Name
                        }),
                    typeof(IExpressionProcessor).Name),
            };
        }
       
        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        //protected override async Task RunAsync(CancellationToken cancellationToken)
        //{
        //    // TODO: Replace the following sample code with your own logic 
        //    //       or remove this RunAsync override if it's not needed in your service.

        //    long iterations = 0;

        //    while (true)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

        //        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        //    }
        //}
    }
}
