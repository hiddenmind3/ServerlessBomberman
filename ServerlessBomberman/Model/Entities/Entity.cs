using System;


namespace ServerlessBomberman.Model
{
    public class Entity
    {
        public EntityEnum EntityType { get; set; }
        public DateTime ExpirationTime { get; set; }

        public Entity()
        {
        }

        public Entity(EntityEnum e)
        {
            EntityType = e;
        }
    }
}
