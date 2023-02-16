using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace FunctionsDemo.DurableOrchestratorTest
{
    public class DurableFunction
    {
        private IConfiguration _config = null;

        //Example: Using the dependency injection framework to inject an IConfiguration object into the class
        public DurableFunction(IConfiguration config)
        {
            _config = config;
        }

        [FunctionName("DurableFunction")]
        public async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

            //Example: pulling values from configuration and using them within the application
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello),  _config.GetValue<string>("ReadMe")));
            
            //Wait on a raised event from an external source
            outputs.Add(await context.WaitForExternalEvent<string>("FileArrived"));
            log.LogInformation("FileArrived event should have fired if you're here");

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName(nameof(SayHello))]
        public string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("DurableFunction_HttpStart")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var instanceId = req.RequestUri.ParseQueryString().GetValues("instance");
            // Function input comes from the request content.
            await starter.StartNewAsync("DurableFunction", instanceId[0]);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId[0]);
        }
    }
}