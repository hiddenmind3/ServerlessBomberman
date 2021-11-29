using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ServerlessBomberman.Model;

namespace LocalAppNew
{
    public partial class Form1 : Form
    {
        private SolidBrush WallBrush = new SolidBrush(Color.Black);
        private SolidBrush BreakableWallBrush = new SolidBrush(Color.Gray);
        private SolidBrush BombBrush = new SolidBrush(Color.Red);
        private SolidBrush ExplosionBrush = new SolidBrush(Color.Yellow);
        private SolidBrush EmptyBrush = new SolidBrush(Color.White);
        private SolidBrush ErrorBrush = new SolidBrush(Color.Magenta);
        private SolidBrush[] PlayerBrush = new SolidBrush[] { new SolidBrush(Color.DarkBlue), new SolidBrush(Color.Blue), new SolidBrush(Color.LightBlue) };
        private static Boolean gameIsRunning = true;
        
        private Rectangle[] rectPlayer;
        private Rectangle[,] rectMap;

        private int rectWidth = 111;
        private int rectHeight = 111;
        
        private static String serverURL = "https://serverlessbomberman.azurewebsites.net/api/";
        private static String serverURLReset = serverURL+"reset/";
        private static String serverURLInput = serverURL+"input/";
        private static String serverURLGetGameState = serverURL+"getgamestate/";

        private String gameKey = "testKey";
        private String playerKey = "playerKey";

        private Game game = new Game();
        private HttpClient client;

        public Form1()
        {

            game.ResetMap();
            client = new HttpClient();
            resetGameOnServer();

            initPlayers();

            InitializeComponent();

            initRectangles();

            putInfo(serverURLInput + gameKey, JsonConvert.SerializeObject(new Input(playerKey, InputEnum.None)));
            Thread.Sleep(500);
            RT(() => {
                updateMap();
            }, 10);
        }

        private void updateMap()
        {
            getGameStateFromServer();
            Invalidate();
        }

        private void initRectangles()
        {
            initMapRectangles();

            initPlayerRectangles();
        }

        private void initPlayerRectangles()
        {
            rectPlayer = new Rectangle[1];
            for (int i = 0; i < game.Players.Count; i++)
            {
                rectPlayer[i] = new Rectangle(i * rectWidth, i * rectHeight, rectWidth, rectHeight);
            }
        }

        private void initMapRectangles()
        {
            rectMap = new Rectangle[game.Map.Length, game.Map[0].Length];
            for (int i = 0; i < game.Map.Length; i++)
            {
                for (int j = 0; j < game.Map[i].Length; j++)
                {
                    rectMap[i, j] = new Rectangle(i * rectWidth, j * rectHeight, rectWidth, rectHeight);
                }
            }
        }

        private void initPlayers()
        {
            game.Players = new List<Player>();
            (int, int) xy = game.GetFreeSpawnLocation();
            game.Players.Add(new Player(playerKey, xy.Item1, xy.Item2));
        }

        private async void putInfo(String url, String message)
        {
            _ = await client.PostAsync(url, new StringContent(message, Encoding.UTF8, "application/json"));
        }

        private async void resetGameOnServer()
        {
            var response = await client.GetAsync(serverURLReset + gameKey);
        }
        private async void getGameStateFromServer()
        {
            var response = await client.GetAsync(serverURLGetGameState + gameKey);

            String jgame = await response.Content.ReadAsStringAsync();

            game = JsonConvert.DeserializeObject<Game>(jgame);
        }

        private void communicate(InputEnum inp)
        {
            putInfo(serverURLInput + gameKey, JsonConvert.SerializeObject(new Input(playerKey, inp)));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            drawRectanglesMap(g);

            drawRectanglesPlayer(g);
        }

        private void drawRectanglesPlayer(Graphics g)
        {
            rectPlayer = new Rectangle[game.Players.Count];
            for (int i = 0; i < game.Players.Count; i++)
            {
                rectPlayer[i] = new Rectangle(game.Players[i].XPosition * rectWidth, game.Players[i].YPosition * rectHeight, rectWidth, rectHeight);

                if (rectPlayer == null || rectPlayer[i] == null)
                {
                    throw new Exception("No Players found!");
                }
                else
                {
                    g.FillRectangle(PlayerBrush[i], rectPlayer[i]);
                    g.DrawRectangle(Pens.Black, rectPlayer[i]);
                }
            }
        }

        private void drawRectanglesMap(Graphics g)
        {
            for (int i = 0; i < game.Map.Length; i++)
            {
                for (int j = 0; j < game.Map[i].Length; j++)
                {
                    Entity en = game.Map[i][j];

                    if (en == null)
                    {
                        g.FillRectangle(ErrorBrush, rectMap[i, j]);
                    }
                    else
                    {
                        switch (en.EntityType)
                        {
                            case EntityEnum.BreakableWall:
                                g.FillRectangle(BreakableWallBrush, rectMap[i, j]);
                                break;
                            case EntityEnum.UnbreakableWall:
                                g.FillRectangle(WallBrush, rectMap[i, j]);
                                break;
                            case EntityEnum.Bomb:
                                g.FillRectangle(BombBrush, rectMap[i, j]);
                                break;
                            case EntityEnum.Explosion:
                                g.FillRectangle(ExplosionBrush, rectMap[i, j]);
                                break;
                            default:
                                g.FillRectangle(EmptyBrush, rectMap[i, j]);
                                break;
                        }
                    }
                    g.DrawRectangle(Pens.Black, rectMap[i, j]);
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
                    communicate(InputEnum.Up);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    communicate(InputEnum.Down);
                }
                else if (e.KeyCode == Keys.Right)
                {
                    communicate(InputEnum.Right);
                }
                else if (e.KeyCode == Keys.Left)
                {
                    communicate(InputEnum.Left);
                }
                else if (e.KeyCode == Keys.Space)
                {
                    communicate(InputEnum.Bomb);
                }
                updateMap();
                e.Handled = true;
            }
        }

        static void RT(Action action, int millis)
        {
            if (action == null)
                return; Task.Run(async () => {
                    while (gameIsRunning)
                    {
                        action();
                        await Task.Delay(TimeSpan.FromMilliseconds(millis));
                    }
                });
        }
    }
}
