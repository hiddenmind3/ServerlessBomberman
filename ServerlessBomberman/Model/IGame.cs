﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessBomberman.Model
{
    interface IGame
    {
        public void ProcessInput(Input input);
    }
}
