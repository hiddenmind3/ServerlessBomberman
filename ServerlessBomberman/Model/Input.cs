using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessBomberman.Model
{
    class Input
    {
        public string EntityKey { get; set; }
        public int Distance { get; set; }

        public Input(string key, int dist)
        {
            EntityKey = key;
            Distance = dist;
        }
    }
}
