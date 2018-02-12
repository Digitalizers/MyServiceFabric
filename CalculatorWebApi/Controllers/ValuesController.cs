using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using Yajat.Digitalizers.Calculator.Contracts;

namespace CalculatorWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ICalculator _calculator;
        public ValuesController()
        {
            var factory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());

            _calculator = factory.CreateServiceProxy<ICalculator>(new Uri("fabric:/MyServiceFabric/CalculatorService"),
                new ServicePartitionKey(),
                TargetReplicaSelector.Default,
                "FooListener");
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {            
            return Ok(await _calculator.Add(2,3));
        }

        // GET api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
