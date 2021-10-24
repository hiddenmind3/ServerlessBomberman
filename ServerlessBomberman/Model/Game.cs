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
        public int Position { get; set; }
        public string id { get; set; }

        public void Move(int dist)
        {
            this.Position += dist;
        }

        public void New(int startPosition, string startId)
        {
            id = startId;
            Position = startPosition;
        }

        /*
        public Task Reset()
        {
            this.Position = 0;
            return Task.CompletedTask;
        }

        public Task<int> Get()
        {
            return Task.FromResult(this.Position);
        }

        public void Delete()
        {
            Entity.Current.DeleteState();
        }
        */

        [FunctionName(nameof(Game))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Game>();
    }
}
