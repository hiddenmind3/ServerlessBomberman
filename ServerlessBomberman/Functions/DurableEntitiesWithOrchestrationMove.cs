using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using ServerlessBomberman.Model;

namespace ServerlessBomberman.Functions
{
    public static class DurableEntitiesWithOrchestrationMove

        /* var entityId = new EntityId(nameof(Counter), "myCounter");

    // Two-way call to the entity which returns a value - awaits the response
    int currentValue = await context.CallEntityAsync<int>(entityId, "Get");
    if (currentValue < 10)
    {
        // One-way signal to the entity which updates the value - does not await a response
        context.SignalEntity(entityId, "Add", 1);
    }*/

    {
        [FunctionName("DurableEntitiesWithOrchestrationMove")]
        public static async Task<int> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            Input input = context.GetInput<Input>();

            var entityId = new EntityId(nameof(EntityGame), input.EntityKey);

            context.SignalEntity(entityId, "Move", input.Distance);
            
            var pos = await context.CallEntityAsync<int>(entityId, "get_position");

            await context.CallEntityAsync<int>(entityId, "get_position");

            return pos;
        }

        [FunctionName("DurableEntitiesWithOrchestrationMove_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "move2/{entityKey}/{dist}")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            string entityKey,
            string dist,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            int distInt = int.Parse(dist);

            var instanceId = await starter.StartNewAsync<Input>("DurableEntitiesWithOrchestrationMove", new Input(entityKey, distInt));

            return await starter.WaitForCompletionOrCreateCheckStatusResponseAsync(req,instanceId);
        }
    }
}