using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessBomberman.Model
{
    public interface IGame
    {
        public void ProcessInput(Input input);
    }
}
