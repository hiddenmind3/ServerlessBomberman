using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessBomberman.Model
{
    static class StaticGame
    {
        private static int position;

        public static void Move(int dist)
        {
            position += dist;
        }

        public static int getPosition()
        {
            return position;
        }

        public static void setPosition(int p)
        {
            position = p;
        }
    }
}
