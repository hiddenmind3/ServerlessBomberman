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

            string id = req.Query["id"];

            string distQuery = req.Query["dist"];
            int dist = int.Parse(distQuery);

            if (game == null)
            {
                game = new Game(0, id);
                log.LogInformation($"Document not found. New Document created");
            }
            else
            {
                log.LogInformation($"Found Document, Id:{game.id}, Position={game.position}");
            }

            game.Move(dist);

            await gameOut.AddAsync(game);

            return new OkObjectResult(game);
        }
    }
}
