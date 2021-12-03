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
            loginForm();
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

        private void loginForm()
        {
            string value1 = "Username";
            string value2 = "Server";
            DialogResult d = LoginBox("Login", "Name and Server:", ref value1, ref value2);
            if (d == DialogResult.OK)
            {
                playerKey = value1;
                gameKey = value2;
            } else if (d == DialogResult.Cancel)
            {
                System.Windows.Forms.Application.Exit();
            }
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
                    rectMap[j, i] = new Rectangle(i * rectWidth, j * rectHeight, rectWidth, rectHeight);
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
                if (game.Players[i].IsAlive)
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
        }

        private void drawRectanglesMap(Graphics g)
        {
            for (int i = 0; i < game.Map.Length; i++)
            {
                for (int j = 0; j < game.Map[i].Length; j++)
                {
                    Entity en = game.Map[j][i];

                    if (en == null)
                    {
                        g.FillRectangle(ErrorBrush, rectMap[j, i]);
                    }
                    else
                    {
                        switch (en.EntityType)
                        {
                            case EntityEnum.BreakableWall:
                                g.FillRectangle(BreakableWallBrush, rectMap[j ,i]);
                                break;
                            case EntityEnum.UnbreakableWall:
                                g.FillRectangle(WallBrush, rectMap[j, i]);
                                break;
                            case EntityEnum.Bomb:
                                g.FillRectangle(BombBrush, rectMap[j, i]);
                                break;
                            case EntityEnum.Explosion:
                                g.FillRectangle(ExplosionBrush, rectMap[j, i]);
                                break;
                            default:
                                g.FillRectangle(EmptyBrush, rectMap[j, i]);
                                break;
                        }
                    }
                    g.DrawRectangle(Pens.Black, rectMap[j, i]);
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
        public static DialogResult LoginBox(string title, string promptText, ref string valueUsername, ref string valueServer)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBoxUsername = new TextBox();
            TextBox textBoxServer = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBoxUsername.Text = valueUsername;
            textBoxServer.Text = valueServer;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Exit";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 10, 382, 13);
            textBoxUsername.SetBounds(12, 36, 372, 20);
            textBoxServer.SetBounds(12, 66, 372, 20);
            buttonOk.SetBounds(173, 97, 75, 30);
            buttonCancel.SetBounds(254, 97, 75, 30);

            label.AutoSize = true;
            textBoxUsername.Anchor = textBoxUsername.Anchor | AnchorStyles.Right;
            textBoxServer.Anchor = textBoxServer.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 130);
            form.Controls.AddRange(new Control[] { label, textBoxUsername, buttonOk, buttonCancel });
            form.Controls.AddRange(new Control[] { label, textBoxServer, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            valueUsername = textBoxUsername.Text;
            valueServer = textBoxServer.Text;
            return dialogResult;
        }
    }
}
