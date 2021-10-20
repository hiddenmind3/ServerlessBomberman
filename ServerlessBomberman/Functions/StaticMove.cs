using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServerlessBomberman.Model;
using Newtonsoft.Json;

namespace ServerlessBomberman.Functions
{
    public static class StaticMove
    {
        [FunctionName("StaticMove")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if(req.Query["reset"] == "1")
            {
                StaticGame.setPosition(0);
                return new OkObjectResult(StaticGame.getPosition());
            }

            string distQuery = req.Query["dist"];
            int dist = int.Parse(distQuery);

            StaticGame.Move(dist);

            return new OkObjectResult(StaticGame.getPosition());
            
            /*
            string name = req.Query["dist"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
            */
        }
    }
}
