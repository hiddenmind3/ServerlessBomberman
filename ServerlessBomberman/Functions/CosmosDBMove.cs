using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServerlessBomberman.Model;
using System.Threading.Tasks;

namespace ServerlessBomberman.Functions
{
    public static class CosmosDBMove
    {
        [FunctionName("CosmosDBMove")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "serverlessbombermancosmosdb",
                collectionName: "games",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{Query.id}",
                PartitionKey = "{Query.id}")] Game game,
            [CosmosDB(
                databaseName: "serverlessbombermancosmosdb",
                collectionName: "games",
                ConnectionStringSetting = "CosmosDBConnection")]IAsyncCollector<dynamic> gameOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string distQuery = req.Query["dist"];
            int dist = int.Parse(distQuery);

            string id = req.Query["id"];

            if (game == null)
            {
                log.LogInformation($"ToDo item not found");
                game = new Game();
                game.New(0, id);
            }
            else
            {
                log.LogInformation($"Found game, Position={game.Position}");
            }

            game.Move(dist);
            var currentPosition = game.Position;

            await gameOut.AddAsync(game);

            return new OkObjectResult(currentPosition);
        }
    }
}
