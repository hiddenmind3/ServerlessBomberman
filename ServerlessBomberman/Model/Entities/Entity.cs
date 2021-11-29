using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessBomberman.Model
{
    public class Entity
    {
        public EntityEnum EntityType { get; set; }
        public int Timer { get; set; }

        public Entity()
        {
        }

        public Entity(EntityEnum e)
        {
            EntityType = e;
        }
    }
}
