using System;
using System.Collections.Generic;
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
        private SolidBrush PlayerBrush = new SolidBrush(Color.Blue);
        private SolidBrush WallBrush = new SolidBrush(Color.Black);
        private SolidBrush BreakableWallBrush = new SolidBrush(Color.Gray);
        private SolidBrush EmptyBrush = new SolidBrush(Color.White);

        private Rectangle[] rectPlayer;
        private Rectangle[,] rectMap;

        private int rectWidth = 111;
        private int rectHeight = 111;
        private static String serverURL = "https://serverlessbomberman.azurewebsites.net/api/";

        private String gameKey = "testKey";
        private String playerKey = "playerKey";

        private Game game = new Game();
        private HttpClient client;

        public Form1()
        {
            game.ResetMap();
            client = new HttpClient();
            _ = getInfo(serverURL + "reset/" + gameKey);

            game.Players = new List<Player>();
            game.Players.Add(new Player(playerKey, 1, 1));

            InitializeComponent();
            rectMap = new Rectangle[game.Map.Length, game.Map[0].Length];
            for (int i = 0; i < game.Map.Length; i++)
            {
                for (int j = 0; j < game.Map[i].Length; j++)
                {
                    rectMap[i, j] = new Rectangle(i * rectWidth, j * rectHeight, rectWidth, rectHeight);
                }
            }
            rectPlayer = new Rectangle[1];
            for (int i = 0; i < game.Players.Count; i++)
            {
                rectPlayer[i] = new Rectangle(i * rectWidth, i * rectHeight, rectWidth, rectHeight);
            }
        }

        private async void putInfo(String url, String message)
        {
            _ = await client.PostAsync(url, new StringContent(message, Encoding.UTF8, "application/json"));
        }

        private async Task<string> getInfo(String url)
        {
            var response = await client.GetAsync(url);

            String gameJsonString = await response.Content.ReadAsStringAsync();

            return gameJsonString;
        }

        private void communicate(InputEnum inp)
        {
            //putInfo(serverURL + "input/" + gameKey, JsonConvert.SerializeObject(new Input(playerKey, inp)));
            var jgame = getInfo(serverURL + "getgamestate/" + gameKey);
            game = JsonConvert.DeserializeObject<Game>(jgame.Result);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            // draw map
            for (int i = 0; i < game.Map.Length; i++)
            {
                for (int j = 0; j < game.Map[i].Length; j++)
                {
                    EntityEnum en = game.Map[i][j];
                    
                    if (en == null)
                    {
                        g.FillRectangle(EmptyBrush, rectMap[i, j]);
                    }
                    else
                    {
                        Type t = en.GetType();
                        switch (en)
                        {
                            case EntityEnum.BreakableWall:
                                g.FillRectangle(BreakableWallBrush, rectMap[i, j]);
                                break;
                            case EntityEnum.UnbreakableWall:
                                g.FillRectangle(WallBrush, rectMap[i, j]);
                                break;
                            case EntityEnum.empty:
                            default:
                                g.FillRectangle(EmptyBrush, rectMap[i, j]);
                                break;
                        }
                    }
                    g.DrawRectangle(Pens.Black, rectMap[i, j]);
                }
            }
            
            // draw players
            for (int i = 0; i < game.Players.Count; i++)
            {
                rectPlayer[i] = new Rectangle(game.Players[i].XPosition * rectWidth, game.Players[i].YPosition * rectHeight, rectWidth, rectHeight);

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
                    g.FillRectangle(PlayerBrush, rectPlayer[i]);
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
