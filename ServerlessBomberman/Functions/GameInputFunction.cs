using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ServerlessBomberman.Model;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading;

namespace ServerlessBomberman.Functions
{
    public static class GameInputFunction
    {
        [FunctionName("GameInputFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "input/{gameKey}")] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client,
            String gameKey,
            ILogger log)
        {
            log.LogInformation("Started GameInputFunction");

            var gameId = new EntityId(nameof(Game), gameKey);

            log.LogInformation("Created gameId");

            var content = await req.Content.ReadAsStringAsync();

            Input input = JsonConvert.DeserializeObject<Input>(content);

            log.LogInformation("Deserialized the Content");

            await client.SignalEntityAsync<IGame>(gameId, game => game.ProcessInput(input));

            return new OkObjectResult("ok");
        }
    }
}
