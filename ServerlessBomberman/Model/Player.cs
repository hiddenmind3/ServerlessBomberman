using System;

namespace ServerlessBomberman.Model
{
    public class Player
    {
        public String Name { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public Boolean IsAlive { get; set; }

        public Player()
        {
        }

        public Player(String name, int xPosition, int yPosition)
        {
            Name = name;
            XPosition = xPosition;
            YPosition = yPosition;
            IsAlive = true;
        }
    }
}
