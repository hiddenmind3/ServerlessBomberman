using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ServerlessBomberman.Model;
using System.Text.Json;
using System.Net.Http;

namespace ServerlessBomberman.Functions
{
    public static class GameInputFunction
    {
        [FunctionName("GameInputFunction")]
        public static async void Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "input/{entityKey}")] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client,
            String gameKey,
            ILogger log)
        {
            var gameId = new EntityId(nameof(Game), gameKey);

            Input input = JsonSerializer.Deserialize<Input>(await req.Content.ReadAsStringAsync());

            await client.SignalEntityAsync<IGame>(gameId, game => game.ProcessInput(input));
        }
    }
}
