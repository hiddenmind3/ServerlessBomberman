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
        public async void TestGameInputFunction()
        {
            var client = new HttpClient();

            Input input = new Input("player_00", InputEnum.Up);

            var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");

            var postResponse = await client.PostAsync("https://serverlessbomberman.azurewebsites.net/api/input/unitTest00", content);

            Thread.Sleep(5000);

            var response = await client.GetAsync("https://serverlessbomberman.azurewebsites.net/api/getgamestate/unitTest00");

            var gameJsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JsonConvert.DeserializeObject<Game>(gameJsonString);

            Game game = new Game();
            game.ProcessInput(input);

            

            Assert.Equal(game, jsonGame);
        }
    }
}
