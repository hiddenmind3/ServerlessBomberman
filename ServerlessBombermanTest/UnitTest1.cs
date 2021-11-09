using System.Net.Http;
using System.Threading;
using Xunit;
using ServerlessBomberman.Model;
using System.Text;
using Newtonsoft.Json;

namespace ServerlessBombermanTest
{
    public class UnitTest1
    {
        private static readonly string resetURL = "https://serverlessbomberman.azurewebsites.net/api/reset/";
        private static readonly string inputURL = "https://serverlessbomberman.azurewebsites.net/api/input/";
        private static readonly string gamestateURL = "https://serverlessbomberman.azurewebsites.net/api/getgamestate/";

        [Fact]
        public async void TestGameInputFunctionUp()
        {
            var client = new HttpClient();
            Input input = new Input("player_00", InputEnum.Up);
            var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
            string gameKey = "testUp";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input);
            game.Players[0].XPosition = 2;
            game.Players[0].YPosition = 1;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestGameInputFunctionDown()
        {
            var client = new HttpClient();
            Input input = new Input("player_00", InputEnum.Down);
            var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
            string gameKey = "testDown";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input);
            game.Players[0].XPosition = 2;
            game.Players[0].YPosition = 3;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestGameInputFunctionLeft()
        {
            var client = new HttpClient();
            Input input = new Input("player_00", InputEnum.Left);
            var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
            string gameKey = "testLeft";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input);
            game.Players[0].XPosition = 1;
            game.Players[0].YPosition = 2;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestGameInputFunctionRight()
        {
            var client = new HttpClient();
            Input input = new Input("player_00", InputEnum.Right);
            var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
            string gameKey = "testRight";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input);
            game.Players[0].XPosition = 3;
            game.Players[0].YPosition = 2;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestGameInputFunctionBomb()
        {
            var client = new HttpClient();
            Input input = new Input("player_00", InputEnum.Bomb);
            var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
            string gameKey = "testBomb";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input);
            game.Map[2][2] = EntityEnum.Bomb;
            game.Players[0].XPosition = 2;
            game.Players[0].YPosition = 2;

            AssertGameEqual(game, jsonGame);
        }

        private void AssertGameEqual(Game expected, Game actual)
        {
            for (int i = 0; i < expected.Map.Length; i++)
            {
                for (int j = 0; j < expected.Map[i].Length; j++)
                {
                    Assert.Equal(expected.Map[i][j], actual.Map[i][j]);
                }
            }
            for (int i = 0; i < expected.Players.Count; i++)
            {
                Assert.Equal(expected.Players[i].Name, actual.Players[i].Name);
                Assert.Equal(expected.Players[i].XPosition, actual.Players[i].XPosition);
                Assert.Equal(expected.Players[i].YPosition, actual.Players[i].YPosition);
            }
        }
    }
}
