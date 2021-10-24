using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessBomberman.Model
{
    interface IGame
    {
        public string id { get; set; }
        public int position { get; set; }
        void Move(int dist);
    }
}
