using Newtonsoft.Json;
using ServerlessBomberman.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTesting
{
    class Program
    {
        private static String serverURL = "https://serverlessbomberman.azurewebsites.net/api/";
        private static String serverURLReset = serverURL + "reset/";
        private static String serverURLInput = serverURL + "input/";
        private static String serverURLGetGameState = serverURL + "getgamestate/";

        private static String gameKey = "testKey";
        private static String playerKey = "playerKey";

        private static Game game = new Game();
        private static HttpClient client = new HttpClient();

        private static Stopwatch stopwatch = new Stopwatch();
        private static int n = 100;

        static void Main(string[] args)
        {
            performanceTestInput(InputEnum.Up);
            performanceTestInput(InputEnum.Up);
            performanceTestInput(InputEnum.Down);
            performanceTestInput(InputEnum.Left);
            performanceTestInput(InputEnum.Right);
            performanceTestInput(InputEnum.Bomb);
            performanceTestReset();
            performanceTestGetGameState();
        }

        private static void performanceTestGetGameState()
        {
            var reset1 = resetGameOnServer();
            reset1.Wait();
            do
            {
                var gameState1 = getGameStateFromServer();
                gameState1.Wait();
            } while (game.Players.Count != 0);
            communicate(InputEnum.None);
            do
            {
                var gameState2 = getGameStateFromServer();
                gameState2.Wait();
            } while (game.Players.Count == 0);
            List<long> times = new List<long>();
            for (int i = 0; i < n; i++)
            {
                stopwatch.Restart();
                var gameState = getGameStateFromServer();
                gameState.Wait();
                stopwatch.Stop();
                times.Add(stopwatch.ElapsedMilliseconds);
            }
            Debug.WriteLine("\nTime Reset = " + Queryable.Average(times.AsQueryable()) + "\n\n");
        }

        private static void performanceTestReset()
        {
            var reset1 = resetGameOnServer();
            reset1.Wait();
            do
            {
                var gameState = getGameStateFromServer();
                gameState.Wait();
            } while (game.Players.Count != 0);
            List<long> times = new List<long>();
            for (int i = 0; i < n; i++)
            {
                stopwatch.Restart();
                var reset2 = resetGameOnServer();
                reset2.Wait();
                do
                {
                    var gameState = getGameStateFromServer();
                    gameState.Wait();
                } while (game.Players.Count != 0);
                stopwatch.Stop();
                times.Add(stopwatch.ElapsedMilliseconds);
                communicate(InputEnum.None);
                do
                {
                    var gameState = getGameStateFromServer();
                    gameState.Wait();
                } while (game.Players.Count == 0); 
            }
            Debug.WriteLine("\nTime Reset = " + Queryable.Average(times.AsQueryable()) + "\n\n");
        }

        private static void performanceTestInput(InputEnum input)
        {
            List<long> times = new List<long>();
            for(int i = 0; i < n; i++)
            {
                var reset = resetGameOnServer();
                reset.Wait();
                do
                {
                    var gameState = getGameStateFromServer();
                    gameState.Wait();
                } while (game.Players.Count != 0);
                stopwatch.Restart();
                communicate(input);
                do
                {
                    var gameState = getGameStateFromServer();
                    gameState.Wait();
                } while (!checkIfInputHasHappened(input));
                stopwatch.Stop();
                times.Add(stopwatch.ElapsedMilliseconds);
            }
            Debug.WriteLine("\nTime " + input.ToString() + " = " + Queryable.Average(times.AsQueryable()) + "\n\n");
        }

        private static Boolean checkIfInputHasHappened(InputEnum input)
        {
            try
            {
                if (game.GetPlayer(playerKey) == null) return false;
                switch (input)
                {
                    case InputEnum.Up:
                        if (game.GetPlayer(playerKey).YPosition == 1)
                        {
                            return true;
                        }
                        break;
                    case InputEnum.Down:
                        if (game.GetPlayer(playerKey).YPosition == 3)
                        {
                            return true;
                        }
                        break;
                    case InputEnum.Left:
                        if (game.GetPlayer(playerKey).XPosition == 1)
                        {
                            return true;
                        }
                        break;
                    case InputEnum.Right:
                        if (game.GetPlayer(playerKey).XPosition == 3)
                        {
                            return true;
                        }
                        break;
                    case InputEnum.Bomb:
                        if (game.Map[2][2].EntityType == EntityEnum.Bomb)
                        {
                            return true;
                        }
                        break;
                }
            return false;
            } catch (Exception) { return false; }

        }

        private static async Task getGameStateFromServer()
        {
            var response = await client.GetAsync(serverURLGetGameState + gameKey);

            String jgame = await response.Content.ReadAsStringAsync();

            game = JsonConvert.DeserializeObject<Game>(jgame);
        }

        private static void communicate(InputEnum inp)
        {
            var input = putInfo(serverURLInput + gameKey, JsonConvert.SerializeObject(new Input(playerKey, inp)));
            input.Wait();
        }

        private static async Task putInfo(String url, String message)
        {
            _ = await client.PostAsync(url, new StringContent(message, Encoding.UTF8, "application/json"));
        }

        private static async Task resetGameOnServer()
        {
            _ = await client.GetAsync(serverURLReset + gameKey);
        }
    }
}
