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
            Player currentPlayer = getCurrentPlayer(input.PlayerName);

            if (currentPlayer == null)
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

        private Player getCurrentPlayer(string playerName)
        {
            foreach (Player player in players)
            {
                if (playerName == player.Name)
                {
                    return player;
                }
            }
            return null;
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
