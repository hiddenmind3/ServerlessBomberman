using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Net.Http;
using ServerlessBomberman.Model;

namespace ServerlessBomberman.Functions
{
    public static class GameResetFunction
    {
        [FunctionName("GameResetFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reset/{gameKey}")] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client,
            String gameKey,
            ILogger log)
        {
            var gameId = new EntityId(nameof(Game), gameKey);

            await client.SignalEntityAsync<IGame>(gameId, game => game.Reset());

            return new OkObjectResult("ok");
        }
    }
}
