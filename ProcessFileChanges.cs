// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.EventGrid;

namespace DurableFunction
{
    public class ProcessFileChanges
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public ProcessFileChanges(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _client = httpClientFactory.CreateClient();
            _config = config;
        }

        [FunctionName("ProcessFileChanges")]
        public void Run([EventGridTrigger]EventGridEvent eventGridEvent, 
        [DurableClient] IDurableOrchestrationClient durableClient, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
            string fileName = eventGridEvent.Subject.Substring(eventGridEvent.Subject.LastIndexOf('/')+1);
            string identifier = fileName.Substring(0, fileName.LastIndexOf('_'));
            log.LogInformation($"Instance ID: {identifier} \nFile Name: {fileName}");
            durableClient.RaiseEventAsync(identifier, "FileArrived", fileName);
        }
    }
}