using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace azureDurableFunctionsExample
{
    public static class DemoDurableFunction
    {
        [FunctionName("HelloSequence")]
        public static async Task<List<string>> Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>
            {
                await context.CallActivityAsync<string>("F1", "Tokyo"),
                await context.CallActivityAsync<string>("F2", "Seattle"),
                await context.CallActivityAsync<string>("F3", "London")
            };

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("F1")]
        public static string F1([ActivityTrigger] IDurableActivityContext context)
        {
            string name = context.GetInput<string>();
            return $"Hello {name}!";
        }

        [FunctionName("F2")]
        public static string F2([ActivityTrigger] IDurableActivityContext context)
        {
            string name = context.GetInput<string>();
            return $"Hello {name}!";
        }

        [FunctionName("F3")]
        public static string F3([ActivityTrigger] IDurableActivityContext context)
        {
            string name = context.GetInput<string>();
            return $"Hello {name}!";
        }

        public static class HttpStart
        {
            [FunctionName("HttpStart")]
            public static async Task<HttpResponseMessage> Run(
                [HttpTrigger(AuthorizationLevel.Function, methods: "post", Route = "orchestrators/{functionName}")] HttpRequestMessage req,
                [DurableClient] IDurableClient starter,
                string functionName,
                ILogger log)
            {
                // Function input comes from the request content.
                object eventData = await req.Content.ReadAsAsync<object>();
                string instanceId = await starter.StartNewAsync(functionName, eventData);

                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

                return starter.CreateCheckStatusResponse(req, instanceId);
            }
        }
    }
}


