using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessBomberman.Model
{
    class EntityGame : IEntityGame
    {
        public int position { get; set; }

        public void Move(int dist)
        {
            this.position += dist;
        }

        public Task Reset(int startPosition) {
            this.position = startPosition;
            return Task.CompletedTask;
        }

        public void Delete()
        {
            Entity.Current.DeleteState();
        }

        public Task<int> GetPosition()
        {
            return Task.FromResult(this.position);
        }


        [FunctionName(nameof(EntityGame))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<EntityGame>();
    }
}
