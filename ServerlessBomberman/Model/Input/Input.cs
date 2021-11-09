using Newtonsoft.Json;
using System;

namespace ServerlessBomberman.Model
{
    public class Input
    {
        public InputEnum PlayerInput { get; set; }
        public String PlayerName { get; set; }

        public Input(String playerName, InputEnum playerInput)
        {
            PlayerName = playerName;
            PlayerInput = playerInput;
        }

        public Input()
        {

        }
    }
}
