using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessBomberman.Model
{
    public interface IEntityGame
    {
        void Move(int dist);
        Task Reset(int startPosition);
        Task<int> GetPosition();
        void Delete();
    }
}
