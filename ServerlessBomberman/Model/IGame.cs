using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessBomberman.Model
{
    interface IGame
    {
        void Move(int dist);
        void New(int startPosition, string startId);

        /*
        Task Reset();
        Task<int> Get();
        void Delete();
        */
    }
}
