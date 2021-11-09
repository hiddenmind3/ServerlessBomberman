using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessBomberman.Model
{
    public class TemporaryEntity : Entity
    {
        public int Timer { get; }
        public TemporaryEntity()
        {
            Timer = 5;
        }
    }
}
