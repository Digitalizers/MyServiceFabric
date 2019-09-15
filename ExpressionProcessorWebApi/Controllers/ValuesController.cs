using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Yajat.Digitalizers.AspStd.ServiceFabric.Proxy;
using Yajat.Digitalizers.ExpressionProcessor.Contracts;

namespace ExpressionProcessorWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values

        public async Task<IActionResult> GetAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                IExpressionProcessor proxy = Proxy.ForComponentDirect<IExpressionProcessor>();
            string result = await proxy.EvaluateExpressionAsync("10-3");
            if (!string.IsNullOrWhiteSpace(result))
            {
                return Ok(result);
            }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
            }
            return BadRequest(HttpStatusCode.BadRequest);
        }
        /*[HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }*/

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
