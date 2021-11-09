using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ServerlessBomberman.Model;
using System.Net.Http;

namespace ServerlessBomberman.Functions
{
    public static class GetGameStateFunction
    {
        [FunctionName("GetGameStateFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getgamestate/{gameKey}")] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client,
            string gameKey,
            ILogger log)
        {
            var gameId = new EntityId(nameof(Game), gameKey);

            var state = await client.ReadEntityStateAsync<Game>(gameId);

            return new OkObjectResult(state.EntityState);
        }
    }
}
