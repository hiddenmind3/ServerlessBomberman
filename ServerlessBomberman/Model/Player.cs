using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessBomberman.Model
{
    class Player
    {
        public String Name { get; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }

        public Player(String name)
        {
            Name = name;
        }

        public Player(String name, int xPosition, int yPosition)
        {
            Name = name;
            XPosition = xPosition;
            YPosition = yPosition;
        }
    }
}
