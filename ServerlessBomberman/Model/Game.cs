using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessBomberman.Model
{
    public class Game : IGame
    {
        public string id { get; set; }

        public int position { get; set; }

        public Game(int startPosition, string startId)
        {
            position = startPosition;
            id = startId;
        }

        public void Move(int dist)
        {
            position += dist;
        }

        [FunctionName(nameof(Game))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Game>();
    }
}
