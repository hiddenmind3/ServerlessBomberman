using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ServerlessBomberman.Model;

namespace ServerlessBomberman.Functions
{
    public static class RemovePlayerFunction
    {
        [FunctionName("RemovePlayerFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "remove/{gameKey}")] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client,
            String gameKey,
            ILogger log)
        {
            var gameId = new EntityId(nameof(Game), gameKey);

            var content = await req.Content.ReadAsStringAsync();

            Input input = JsonConvert.DeserializeObject<Input>(content);

            await client.SignalEntityAsync<IGame>(gameId, game => game.RemovePlayer(input));

            return new OkObjectResult("ok");
        }
    }
}
