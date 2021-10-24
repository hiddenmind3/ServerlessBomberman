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
using Newtonsoft.Json.Linq;
using ServerlessBomberman.Model;
using System.Net.Http;
using System.Net;
using System.Diagnostics;

namespace ServerlessBomberman.Functions
{
    public static class DurableEntitiesMove
    {
        [FunctionName("DurableEntitiesMove")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "move/{entityKey}/{dist}")] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client,
            string entityKey,
            string dist,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            int distInt = int.Parse(dist);

            var entityId = new EntityId(nameof(Game), entityKey);

            await client.SignalEntityAsync<IGame>(entityId, game => new Game(0, "testId"));

            await client.SignalEntityAsync<IGame>(entityId, game => game.Move(distInt));

            var state = await client.ReadEntityStateAsync<Game>(entityId);

            return req.CreateResponse(state);
        }
    }
}
