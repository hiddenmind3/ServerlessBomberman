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
        // https://serverlessbomberman.azurewebsites.net for Azure Testing
        // http://localhost:7071 for Local Testing
        private static readonly string resetURL = "https://serverlessbomberman.azurewebsites.net/api/reset/";
        private static readonly string inputURL = "https://serverlessbomberman.azurewebsites.net/api/input/";
        private static readonly string gamestateURL = "https://serverlessbomberman.azurewebsites.net/api/getgamestate/";
        private static readonly string removePlayerURL = "https://serverlessbomberman.azurewebsites.net/api/remove/";

        [Fact]
        public async void TestGameInputFunctionNoInput()
        {
            var client = new HttpClient();
            Input input_none_0 = new Input("player_00", InputEnum.None);
            var content_none_0 = new StringContent(JsonConvert.SerializeObject(input_none_0), Encoding.UTF8, "application/json");
            string gameKey = "testNone";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_none_0);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input_none_0);
            game.Players[0].XPosition = 2;
            game.Players[0].YPosition = 2;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestGameInputFunctionUp()
        {
            var client = new HttpClient();
            Input input_up_0 = new Input("player_00", InputEnum.Up);
            Input input_none_0 = new Input("player_00", InputEnum.None);
            var content_up_0 = new StringContent(JsonConvert.SerializeObject(input_up_0), Encoding.UTF8, "application/json");
            string gameKey = "testUp";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_up_0);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input_none_0);
            game.Players[0].XPosition = 2;
            game.Players[0].YPosition = 1;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestGameInputFunctionDown()
        {
            var client = new HttpClient();
            Input input_down_0 = new Input("player_00", InputEnum.Down);
            Input input_none_0 = new Input("player_00", InputEnum.None);
            var content_down_0 = new StringContent(JsonConvert.SerializeObject(input_down_0), Encoding.UTF8, "application/json");
            string gameKey = "testDown";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_down_0);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input_none_0);
            game.Players[0].XPosition = 2;
            game.Players[0].YPosition = 3;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestGameInputFunctionLeft()
        {
            var client = new HttpClient();
            Input input_left_0 = new Input("player_00", InputEnum.Left);
            Input input_none_0 = new Input("player_00", InputEnum.None);
            var content_left_0 = new StringContent(JsonConvert.SerializeObject(input_left_0), Encoding.UTF8, "application/json");
            string gameKey = "testLeft";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_left_0);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input_none_0);
            game.Players[0].XPosition = 1;
            game.Players[0].YPosition = 2;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestGameInputFunctionRight()
        {
            var client = new HttpClient();
            Input input_right_0 = new Input("player_00", InputEnum.Right);
            Input input_none_0 = new Input("player_00", InputEnum.None);
            var content_right_0 = new StringContent(JsonConvert.SerializeObject(input_right_0), Encoding.UTF8, "application/json");
            string gameKey = "testRight";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_right_0);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input_none_0);
            game.Players[0].XPosition = 3;
            game.Players[0].YPosition = 2;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestGameInputFunctionBomb()
        {
            var client = new HttpClient();
            Input input_bomb_0 = new Input("player_00", InputEnum.Bomb);
            Input input_none_0 = new Input("player_00", InputEnum.None);
            var content_bomb_0 = new StringContent(JsonConvert.SerializeObject(input_bomb_0), Encoding.UTF8, "application/json");
            string gameKey = "testBomb";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_bomb_0);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input_none_0);
            game.Map[2][2].EntityType = EntityEnum.Bomb;
            game.Players[0].XPosition = 2;
            game.Players[0].YPosition = 2;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestBombCollisionWithPlayer()
        {
            var client = new HttpClient();
            Input input_bomb_0 = new Input("player_00", InputEnum.Bomb);
            Input input_left_0 = new Input("player_00", InputEnum.Left);
            Input input_right_0 = new Input("player_00", InputEnum.Right);
            Input input_none_0 = new Input("player_00", InputEnum.None);
            var content_bomb_0 = new StringContent(JsonConvert.SerializeObject(input_bomb_0), Encoding.UTF8, "application/json");
            var content_left_0 = new StringContent(JsonConvert.SerializeObject(input_left_0), Encoding.UTF8, "application/json");
            var content_right_0 = new StringContent(JsonConvert.SerializeObject(input_right_0), Encoding.UTF8, "application/json");
            string gameKey = "testBombCollision";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_bomb_0);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_left_0);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_right_0);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input_none_0);
            game.Map[2][2].EntityType = EntityEnum.Bomb;
            game.Players[0].XPosition = 1;
            game.Players[0].YPosition = 2;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestPlayerCollisionWithPlayer()
        {
            var client = new HttpClient();
            Input input_right_0 = new Input("player_00", InputEnum.Right);
            Input input_none_0 = new Input("player_00", InputEnum.None);
            Input input_none_1 = new Input("player_01", InputEnum.None);
            var content_right_0 = new StringContent(JsonConvert.SerializeObject(input_right_0), Encoding.UTF8, "application/json");
            var content_none_0 = new StringContent(JsonConvert.SerializeObject(input_none_0), Encoding.UTF8, "application/json");
            var content_none_1 = new StringContent(JsonConvert.SerializeObject(input_none_1), Encoding.UTF8, "application/json");
            string gameKey = "testPlayerCollision";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_none_0);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_none_1);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_right_0);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_right_0);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input_none_0);
            game.ProcessInput(input_none_1);
            game.Players[0].XPosition = 3;
            game.Players[0].YPosition = 2;
            game.Players[1].XPosition = 4;
            game.Players[1].YPosition = 2;

            AssertGameEqual(game, jsonGame);
        }

        [Fact]
        public async void TestRemovePlayerFunction()
        {
            var client = new HttpClient();
            Input input_none_0 = new Input("player_00", InputEnum.None);
            var content_none_0 = new StringContent(JsonConvert.SerializeObject(input_none_0), Encoding.UTF8, "application/json");
            string gameKey = "testRemovePlayer";

            _ = await client.GetAsync(resetURL + gameKey);
            Thread.Sleep(1000);
            _ = await client.PostAsync(inputURL + gameKey, content_none_0);
            Thread.Sleep(1000);
            _ = await client.PostAsync(removePlayerURL + gameKey, content_none_0);
            Thread.Sleep(1000);
            var response = await client.GetAsync(gamestateURL + gameKey);

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.Reset();

            AssertGameEqual(game, jsonGame);
        }

        private void AssertGameEqual(Game expected, Game actual)
        {
            for (int i = 0; i < expected.Map.Length; i++)
            {
                for (int j = 0; j < expected.Map[i].Length; j++)
                {
                    Assert.Equal(expected.Map[i][j].GetType(), actual.Map[i][j].GetType());
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
