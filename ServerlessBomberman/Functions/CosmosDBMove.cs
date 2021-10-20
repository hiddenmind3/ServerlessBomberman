using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServerlessBomberman.Model;

namespace ServerlessBomberman.Functions
{
    public static class CosmosDBMove
    {
        [FunctionName("CosmosDBMove")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "serverlessbomberman",
                collectionName: "Items",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{Query.id}",
                PartitionKey = "{Query.id}")] Game game,
            [CosmosDB(
                databaseName: "serverlessbomberman",
                collectionName: "Items",
                ConnectionStringSetting = "CosmosDBConnection")]out dynamic gameOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string distQuery = req.Query["dist"];
            int dist = int.Parse(distQuery);

            if (game == null)
            {
                log.LogInformation($"ToDo item not found");
                game = new Game();
                game.New(0);
            }
            else
            {
                log.LogInformation($"Found game, Position={game.Position}");
            }

            game.Move(dist);
            var currentPosition = game.Position;

            gameOut = new { Position = currentPosition };

            return new OkObjectResult(currentPosition);
        }
    }
}
