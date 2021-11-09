using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerlessBomberman.Model
{
    public class Game : IGame
    {
        public EntityEnum[][] Map { get; set; }
        public List<Player> Players { get; set; }

        public void Reset()
        {
            ResetMap();
            ResetPlayers();
        }

        public void ProcessInput(Input input)
        {
            if (Map == null || Players == null) Reset();

            Player currentPlayer = GetCurrentPlayer(input.PlayerName);

            if (currentPlayer == null)
            {
                return;
            }

            ProcessPlayerInput(input, currentPlayer);
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

        private void ResetMap()
        {
            Map = new EntityEnum[7][] {
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall},
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.empty, EntityEnum.empty, EntityEnum.BreakableWall, EntityEnum.empty, EntityEnum.empty, EntityEnum.UnbreakableWall},
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.empty, EntityEnum.emptySpawn, EntityEnum.empty, EntityEnum.emptySpawn, EntityEnum.empty, EntityEnum.UnbreakableWall},
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.BreakableWall, EntityEnum.empty, EntityEnum.BreakableWall, EntityEnum.empty, EntityEnum.BreakableWall, EntityEnum.UnbreakableWall},
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.empty, EntityEnum.emptySpawn, EntityEnum.empty, EntityEnum.emptySpawn, EntityEnum.empty, EntityEnum.UnbreakableWall},
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.empty, EntityEnum.empty, EntityEnum.BreakableWall, EntityEnum.empty, EntityEnum.empty, EntityEnum.UnbreakableWall},
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall, EntityEnum.UnbreakableWall} };
        }

        private Player GetCurrentPlayer(string playerName)
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

        private (int,int) GetFreeSpawnLocation()
        {
            for(int x = 0; x < Map.Length; x++)
            {
                for(int y = 0; y < Map[0].Length; y++)
                {
                    if(Map[x][y] == EntityEnum.emptySpawn && !IsPlayerOnPosition(x,y))
                    {
                        return (x, y);
                    }
                }
            }

            return (-1,-1);
        }

        private bool IsPlayerOnPosition(int x, int y)
        {
            foreach (Player player in Players)
            {
                if (player.XPosition == x && player.YPosition == y)
                {
                    return true;
                }
            }
            return false;
        }

        private void PlaceBomb(int xPosition, int yPosition)
        {
            Map[xPosition][yPosition] = EntityEnum.Bomb;
        }

        private bool CheckIfPositionFree(int xPosition, int yPosition)
        {
            if(Map[xPosition][yPosition] == EntityEnum.empty  || Map[xPosition][yPosition] == EntityEnum.emptySpawn)
            {
                
                return true;
            }
            return false;
        }

        [FunctionName(nameof(Game))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Game>();
    }
}
