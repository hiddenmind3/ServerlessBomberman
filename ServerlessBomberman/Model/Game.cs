using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerlessBomberman.Model
{
    public class Game : IGame
    {
        public Entity[][] Map { get; set; }
        public List<Player> Players { get; set; }

        public void Reset()
        {
            ResetMap();
            ResetPlayers();
        }

        public void ProcessInput(Input input)
        {
            if (Map == null || Players == null) Reset();

            Player currentPlayer = GetOrCreatePlayer(input.PlayerName);

            if (currentPlayer == null)
            {
                return;
            }

            ProcessPlayerInput(input, currentPlayer);
        }

        public void RemovePlayer(Input input)
        {
            Players.Remove(GetOrCreatePlayer(input.PlayerName));
        }

        public void CheckTime()
        {
            for (int x = 0; x < Map.Length; x++)
            {
                for (int y = 0; y < Map[0].Length; y++)
                {
                    if (Map[y][x].EntityType == EntityEnum.Bomb && Map[y][x].ExpirationTime < System.DateTime.Now)
                    {
                        ExplodeTile(y + 1, x);
                        ExplodeTile(y - 1, x);
                        ExplodeTile(y, x);
                        ExplodeTile(y, x + 1);
                        ExplodeTile(y, x - 1);
                    } 
                    else if (Map[y][x].EntityType == EntityEnum.Explosion)
                    {
                        if(Map[y][x].ExpirationTime < System.DateTime.Now)
                        {
                            RemoveExplosion(y, x);
                        }

                        foreach(Player p in Players)
                        {
                            if(p.IsAlive && p.XPosition == x && p.YPosition == y)
                            {
                                p.IsAlive = false;
                            }
                        }
                    }
                }


            }
        }

        private void RemoveExplosion(int y, int x)
        {
            if (Map[y][x].EntityType != EntityEnum.UnbreakableWall)
            {
                Map[y][x].EntityType = EntityEnum.empty;
            }
        }

        private void ExplodeTile(int y, int x)
        {
            if (Map[y][x].EntityType != EntityEnum.UnbreakableWall)
            {
                Map[y][x].EntityType = EntityEnum.Explosion;
                Map[y][x].ExpirationTime = System.DateTime.Now.AddSeconds(1);
            }
        }

        private void ProcessPlayerInput(Input input, Player currentPlayer)
        {
            var x = currentPlayer.XPosition;
            var y = currentPlayer.YPosition;
            switch (input.PlayerInput)
            {
                case InputEnum.Up:
                    if (CheckIfPositionFree(x, y - 1) && !IsPlayerOnPosition(x, y - 1))
                        currentPlayer.YPosition -= 1;
                    break;
                case InputEnum.Down:
                    if (CheckIfPositionFree(x, y + 1) && !IsPlayerOnPosition(x, y + 1))
                        currentPlayer.YPosition += 1;
                    break;
                case InputEnum.Left:
                    if (CheckIfPositionFree(x - 1, y) && !IsPlayerOnPosition(x - 1, y))
                        currentPlayer.XPosition -= 1;
                    break;
                case InputEnum.Right:
                    if (CheckIfPositionFree(x + 1, y) && !IsPlayerOnPosition(x + 1, y))
                        currentPlayer.XPosition += 1;
                    break;
                case InputEnum.Bomb:
                    PlaceBomb(x, y);
                    break;
            }
        }

        private void ResetPlayers()
        {
            Players = new List<Player>();
        }

        public void ResetMap()
        {
            Map = new Entity[7][] {
                new Entity[7]{ new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.UnbreakableWall), 
                    new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.UnbreakableWall)},
                new Entity[7]{ new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.empty), new Entity(EntityEnum.empty), 
                    new Entity(EntityEnum.BreakableWall), new Entity(EntityEnum.empty), new Entity(EntityEnum.empty), new Entity(EntityEnum.UnbreakableWall)},
                new Entity[7]{ new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.empty), new Entity(EntityEnum.emptySpawn),
                    new Entity(EntityEnum.empty), new Entity(EntityEnum.emptySpawn), new Entity(EntityEnum.empty), new Entity(EntityEnum.UnbreakableWall)},
                new Entity[7]{ new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.BreakableWall), new Entity(EntityEnum.empty),
                    new Entity(EntityEnum.BreakableWall), new Entity(EntityEnum.empty), new Entity(EntityEnum.BreakableWall), new Entity(EntityEnum.UnbreakableWall)},
                new Entity[7]{ new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.empty), new Entity(EntityEnum.emptySpawn),
                    new Entity(EntityEnum.empty), new Entity(EntityEnum.emptySpawn), new Entity(EntityEnum.empty), new Entity(EntityEnum.UnbreakableWall)},
                new Entity[7]{ new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.empty), new Entity(EntityEnum.empty),
                    new Entity(EntityEnum.BreakableWall), new Entity(EntityEnum.empty), new Entity(EntityEnum.empty), new Entity(EntityEnum.UnbreakableWall)},
                new Entity[7]{ new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.UnbreakableWall),
                    new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.UnbreakableWall), new Entity(EntityEnum.UnbreakableWall) } };
        }

        private Player GetOrCreatePlayer(string playerName)
        {
            foreach (Player player in Players)
            {
                if (playerName == player.Name)
                {
                    return player;
                }
            }


            (int, int) freeSpawnLocation = GetFreeSpawnLocation();

            if (freeSpawnLocation != (-1, -1))
            {
                var newPlayer = new Player(playerName, freeSpawnLocation.Item1, freeSpawnLocation.Item2);
                Players.Add(newPlayer);
                return newPlayer;
            }

            return null;
        }

        public (int, int) GetFreeSpawnLocation()
        {
            for (int x = 0; x < Map.Length; x++)
            {
                for (int y = 0; y < Map[0].Length; y++)
                {
                    if (Map[y][x].EntityType == EntityEnum.emptySpawn && !IsPlayerOnPosition(x, y))
                    {
                        return (y, x);
                    }
                }
            }

            return (-1, -1);
        }

        private bool IsPlayerOnPosition(int x, int y)
        {
            foreach (Player player in Players)
            {
                if (player.IsAlive && player.XPosition == x && player.YPosition == y)
                {
                    return true;
                }
            }
            return false;
        }

        private void PlaceBomb(int xPosition, int yPosition)
        {
            Map[yPosition][xPosition].EntityType = EntityEnum.Bomb;
            Map[yPosition][xPosition].ExpirationTime = System.DateTime.Now.AddSeconds(5);
        }

        private bool CheckIfPositionFree(int xPosition, int yPosition)
        {
            if(Map[yPosition][xPosition].EntityType == EntityEnum.empty || Map[yPosition][xPosition].EntityType == EntityEnum.emptySpawn || Map[yPosition][xPosition].EntityType == EntityEnum.Explosion)
            {
                return true;
            }
            return false;
        }

        public Player GetPlayer(string name)
        {
            if(Players == null) { return null; }
            foreach(Player player in Players)
            {
                if(player.Name == name)
                {
                    return player;
                }
            }
            return null;
        }

        [FunctionName(nameof(Game))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
                => ctx.DispatchAsync<Game>();
    }
}
