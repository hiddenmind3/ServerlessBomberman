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

            if(currentPlayer == null)
            {
                return;
            }

            switch (input.PlayerInput)
            {
                case InputEnum.Up:
                    if (CheckIfPositionFree(currentPlayer.XPosition, currentPlayer.YPosition-1)) 
                        currentPlayer.YPosition -= 1;
                break;
                case InputEnum.Down:
                    if (CheckIfPositionFree(currentPlayer.XPosition, currentPlayer.YPosition+1))
                        currentPlayer.YPosition += 1;
                break;
                case InputEnum.Left:
                    if (CheckIfPositionFree(currentPlayer.XPosition-1, currentPlayer.YPosition))
                        currentPlayer.XPosition -= 1;
                break;
                case InputEnum.Right:
                    if (CheckIfPositionFree(currentPlayer.XPosition+1, currentPlayer.YPosition))
                        currentPlayer.XPosition += 1;
                break;
                case InputEnum.Bomb:
                    PlaceBomb(currentPlayer.XPosition, currentPlayer.YPosition);
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
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.empty, EntityEnum.empty, EntityEnum.empty, EntityEnum.empty, EntityEnum.empty, EntityEnum.UnbreakableWall},
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.BreakableWall, EntityEnum.UnbreakableWall, EntityEnum.empty, EntityEnum.UnbreakableWall, EntityEnum.BreakableWall, EntityEnum.UnbreakableWall},
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.empty, EntityEnum.empty, EntityEnum.BreakableWall, EntityEnum.empty, EntityEnum.empty, EntityEnum.UnbreakableWall},
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.BreakableWall, EntityEnum.UnbreakableWall, EntityEnum.empty, EntityEnum.UnbreakableWall, EntityEnum.BreakableWall, EntityEnum.UnbreakableWall},
                new EntityEnum[7]{ EntityEnum.UnbreakableWall, EntityEnum.empty, EntityEnum.empty, EntityEnum.empty, EntityEnum.empty, EntityEnum.empty, EntityEnum.UnbreakableWall},
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

            
            (int, int) freeStartPosition = GetFreePosition();

            if (freeStartPosition != (-1, -1))
            {
                var newPlayer = new Player(playerName, freeStartPosition.Item1, freeStartPosition.Item2);
                Players.Add(newPlayer);
                return newPlayer;
            }

            return null;
        }

        private (int,int) GetFreePosition()
        {
            for(int x = 0; x < Map.Length; x++)
            {
                for(int y = 0; y < Map[0].Length; y++)
                {
                    if(Map[x][y] == EntityEnum.empty)
                    {
                        return (x, y);
                    }
                }
            }

            return (-1,-1);
        }

        private void PlaceBomb(int xPosition, int yPosition)
        {
            Map[xPosition][yPosition] = EntityEnum.Bomb;
        }

        private bool CheckIfPositionFree(int xPosition, int yPosition)
        {
            if(Map[xPosition][yPosition] == EntityEnum.empty)
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
