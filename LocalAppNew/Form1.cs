using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ServerlessBomberman.Model;

namespace LocalAppNew
{
    public partial class Form1 : Form
    {
        private Rectangle[] rectPlayer;
        private Rectangle[,] rectMap;
        private int rectWidth = 110;
        private int rectHeight = 110;
        private static String serverURL = "https://serverlessbomberman.azurewebsites.net/api/";

        private String gameKey = "testKey";
        private String playerKey = "playerKey";

        private Game game = new Game();

        public Form1()
        {
            game.ResetMap();

            _ = getInfo(serverURL + "reset/" + gameKey);

            game.players = new Player[] { new Player(playerKey, 1, 1)};
            InitializeComponent();
            rectMap = new Rectangle[game.map.Length, game.map[0].Length];
            for (int i = 0; i < game.map.Length; i++)
            {
                for (int j = 0; j < game.map[i].Length; j++)
                {
                    rectMap[i, j] = new Rectangle(i * rectWidth, j * rectHeight, rectWidth, rectHeight);
                }
            }
            rectPlayer = new Rectangle[1];
            for (int i = 0; i < game.players.Length; i++)
            {
                rectPlayer[i] = new Rectangle(i * rectWidth, i * rectHeight, rectWidth, rectHeight);
            }
        }

        private async Task<string> getInfo(String url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);

            String gameJsonString = await response.Content.ReadAsStringAsync();
            return gameJsonString;
        }

        private async void putInfo(String url, String message)
        {
            var client = new HttpClient();
            _ = await client.PostAsync(url, new StringContent(message, Encoding.UTF8, "application/json"));
        }

        private void communicate(InputEnum inp)
        {
            putInfo(serverURL + "input/" + gameKey, JsonConvert.SerializeObject(new Input(playerKey, inp)));
            game = JsonConvert.DeserializeObject<Game>(getInfo(serverURL + "getgamestate/" + gameKey).Result);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            SolidBrush playerBrush = new SolidBrush(Color.Blue);
            SolidBrush WallBrush = new SolidBrush(Color.Black);
            SolidBrush BreakableWallBrush = new SolidBrush(Color.Gray);
            SolidBrush EmptyBrush = new SolidBrush(Color.White);

            // draw map
            for (int i = 0; i < game.map.Length; i++)
            {
                for (int j = 0; j < game.map[i].Length; j++)
                {
                    Entity en = game.map[i][j];

                    if (en == null)
                    {
                        g.FillRectangle(EmptyBrush, rectMap[i, j]);
                    }
                    else
                    {
                        Type t = en.GetType();
                        if (t.Equals(typeof(BreakableWall)))
                        {
                            g.FillRectangle(BreakableWallBrush, rectMap[i, j]);
                        }
                        else if (t.Equals(typeof(UnbreakableWall)))
                        {
                            g.FillRectangle(WallBrush, rectMap[i, j]);
                        }
                        else if (t.Equals(typeof(BreakableWall)))
                        {
                            g.FillRectangle(BreakableWallBrush, rectMap[i, j]);
                        }
                        else
                        {
                            g.FillRectangle(EmptyBrush, rectMap[i, j]);
                        }
                    }
                    g.DrawRectangle(Pens.Black, rectMap[i, j]);
                }
            }
            
            // draw players
            for (int i = 0; i < game.players.Length; i++)
            {
                rectPlayer[i] = new Rectangle(game.players[i].XPosition * rectWidth, game.players[i].YPosition * rectHeight, rectWidth, rectHeight);

                if (rectPlayer == null || rectPlayer[i] == null)
                {

                } else {
                    switch (i)
                    {
                        case 0:
                            g.FillRectangle(EmptyBrush, rectPlayer[i]);
                            break;
                        case 1:
                            g.FillRectangle(WallBrush, rectPlayer[i]);
                            break;
                        case 2:
                            g.FillRectangle(BreakableWallBrush, rectPlayer[i]);
                            break;
                    }

                    g.DrawRectangle(Pens.Green, rectPlayer[i]);
                    g.FillRectangle(playerBrush, rectPlayer[i]);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                if (e.KeyCode == Keys.Up)
                {
                    //rect.Offset(0, -dy);
                    communicate(InputEnum.Up);
                    //BackColor = Color.FromName("Blue");
                    //MessageBox.Show("You pressed the Up key.");
                }
                else if (e.KeyCode == Keys.Down)
                {
                    //rect.Offset(0, dy);
                    communicate(InputEnum.Down);
                    //BackColor = Color.FromName("Red");
                    //MessageBox.Show("You pressed the Down key.");
                }
                else if (e.KeyCode == Keys.Right)
                {
                    //rect.Offset(dx, 0);
                    communicate(InputEnum.Right);
                    //BackColor = Color.FromName("Green");
                    //MessageBox.Show("You pressed the Right key.");
                }
                else if (e.KeyCode == Keys.Left)
                {
                    //rect.Offset(-dx, 0);
                    communicate(InputEnum.Left);
                    //BackColor = Color.FromName("Yellow");
                    //MessageBox.Show("You pressed the Left key.");
                }
                Invalidate();
                e.Handled = true;
            }
        }
    }
}
