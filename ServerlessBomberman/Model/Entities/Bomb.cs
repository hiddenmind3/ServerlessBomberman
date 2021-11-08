using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessBomberman.Model
{
    class Bomb : Entity
    {
        public int Timer { get; }
        public Bomb()
        {
            Timer = 5;
        }
    }
}
