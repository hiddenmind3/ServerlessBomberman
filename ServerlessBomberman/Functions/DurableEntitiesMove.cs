using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ServerlessBomberman.Model;
using System.Net.Http;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace ServerlessBomberman.Functions
{
    public static class DurableEntitiesMove
    {
        [FunctionName("DurableEntitiesMove")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "move/{entityKey}/{dist}")] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client,
            string entityKey,
            string dist,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            int distInt = int.Parse(dist);

            var entityId = new EntityId(nameof(EntityGame), entityKey);

            await client.SignalEntityAsync<IEntityGame>(entityId, game => game.Move(distInt));

            Thread.Sleep(1000);

            var state = await client.ReadEntityStateAsync<EntityGame>(entityId);

            return new OkObjectResult(state);
        }
    }
}
