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
        [Fact]
        public async void TestGameInputUpFunction()
        {
            var client = new HttpClient();

            Input input = new Input("player_00", InputEnum.Down);

            var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");

            _ = await client.PostAsync("https://serverlessbomberman.azurewebsites.net/api/reset/unitTest00", content);

            Thread.Sleep(1000);

            _ = await client.PostAsync("https://serverlessbomberman.azurewebsites.net/api/input/unitTest00", content);

            Thread.Sleep(1000);

            var response = await client.GetAsync("https://serverlessbomberman.azurewebsites.net/api/getgamestate/unitTest00");

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input);
            game.Players[0].XPosition = 1;
            game.Players[0].YPosition = 2;
            assertGameEqual(game, jsonGame);
        }

        private void assertGameEqual(Game expected, Game actual)
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
