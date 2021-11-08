using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessBomberman.Model
{
    class TemporaryEntity : Entity
    {
        public int Timer { get; }
        public TemporaryEntity()
        {
            Timer = 5;
        }
    }
}
