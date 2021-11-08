using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessBomberman.Model
{
    class Game : IGame
    {
        private Entity[][] map;
        private Player[] players;

        public void ProcessInput(Input input)
        {
            if (map == null) ResetMap();
            
            Player currentPlayer = getCurrentPlayer(input.PlayerName);

            if(currentPlayer == null)
            {
                return;
            }

            switch (input.PlayerInput)
            {
                case InputEnum.Up:
                    if (CheckIfPositionFree(currentPlayer.XPosition, currentPlayer.YPosition + 1)) 
                        currentPlayer.YPosition += 1;
                break;
                case InputEnum.Down:
                    if (CheckIfPositionFree(currentPlayer.XPosition, currentPlayer.YPosition-1))
                        currentPlayer.YPosition -= 1;
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

        private void ResetMap()
        {
            map = new Entity[7][] {
                new Entity[7]{ new UnbreakableWall(), new UnbreakableWall(), new UnbreakableWall(), new UnbreakableWall(), new UnbreakableWall(), new UnbreakableWall(), new UnbreakableWall()},
                new Entity[7]{ new UnbreakableWall(), null, null, null, null, null, new UnbreakableWall()},
                new Entity[7]{ new UnbreakableWall(), new BreakableWall(), new UnbreakableWall(), null, new UnbreakableWall(), new BreakableWall(), new UnbreakableWall()},
                new Entity[7]{ new UnbreakableWall(), null, null, new BreakableWall(), null, null, new UnbreakableWall()},
                new Entity[7]{ new UnbreakableWall(), new BreakableWall(), new UnbreakableWall(), null, new UnbreakableWall(), new BreakableWall(), new UnbreakableWall()},
                new Entity[7]{ new UnbreakableWall(), null, null, null, null, null, new UnbreakableWall()},
                new Entity[7]{ new UnbreakableWall(), new UnbreakableWall(), new UnbreakableWall(), new UnbreakableWall(), new UnbreakableWall(), new UnbreakableWall(), new UnbreakableWall()} };
        }

        private Player getCurrentPlayer(string playerName)
        {
            foreach (Player player in players)
            {
                if (playerName == player.Name)
                {
                    return player;
                }
            }

            (int, int) freeStartPosition = getFreePosition();

            if(freeStartPosition != (-1,-1))
            {
                return new Player(playerName, freeStartPosition.Item1, freeStartPosition.Item2);
            }

            return null;
        }

        private (int,int) getFreePosition()
        {
            for(int x = 0; x < map.Length; x++)
            {
                for(int y = 0; y < map[0].Length; y++)
                {
                    if(map[x][y] == null)
                    {
                        return (x, y);
                    }
                }
            }

            return (-1,-1);
        }

        private void PlaceBomb(int xPosition, int yPosition)
        {
            map[xPosition][yPosition] = new Bomb();
        }

        private bool CheckIfPositionFree(int xPosition, int yPosition)
        {
            if(map[xPosition][yPosition] != null)
            {
                return false;
            }
            return true;
        }
    }
}
