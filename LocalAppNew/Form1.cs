using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
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

        private int rectWidth = 111;
        private int rectHeight = 111;

        private TextureBrush[] PlayerBrush = new TextureBrush[] { new TextureBrush(Image.FromFile(@"Images\\PlayerBlue.png"), System.Drawing.Drawing2D.WrapMode.Tile), new TextureBrush(Image.FromFile(@"Images\\PlayerGreen.png"), System.Drawing.Drawing2D.WrapMode.Tile), new TextureBrush(Image.FromFile(@"Images\\PlayerRed.png"), System.Drawing.Drawing2D.WrapMode.Tile), new TextureBrush(Image.FromFile(@"Images\\PlayerYellow.png"), System.Drawing.Drawing2D.WrapMode.Tile) };
        private TextureBrush WallBrush = new TextureBrush(Image.FromFile(@"Images\\SolidWall.png"), System.Drawing.Drawing2D.WrapMode.Tile);
        private TextureBrush BreakableWallBrush = new TextureBrush(Image.FromFile(@"Images\\BreakableWall.png"), System.Drawing.Drawing2D.WrapMode.Tile);
        private TextureBrush BombBrush = new TextureBrush(Image.FromFile(@"Images\\Bomb.png"), System.Drawing.Drawing2D.WrapMode.Tile);
        private TextureBrush ExplosionBrush = new TextureBrush(Image.FromFile(@"Images\\Explosion.png"), System.Drawing.Drawing2D.WrapMode.Tile);
        private TextureBrush EmptyBrush = new TextureBrush(Image.FromFile(@"Images\\Floor.png"), System.Drawing.Drawing2D.WrapMode.Tile);
        private TextureBrush ErrorBrush = new TextureBrush(Image.FromFile(@"Images\\Error.png"), System.Drawing.Drawing2D.WrapMode.Tile);
        
        private static Boolean gameIsRunning = true;
        
        private Rectangle[] rectPlayer;
        private Rectangle[,] rectMap;

        
        private static String serverURL = "https://serverlessbomberman.azurewebsites.net/api/";
        private static String serverURLReset = serverURL+"reset/";
        private static String serverURLInput = serverURL+"input/";
        private static String serverURLGetGameState = serverURL + "getgamestate/";
        private static String serverURLRemovePlayer = serverURL + "remove/";
        
        private String gameKey = "testKey";
        private String playerKey = "playerKey";

        private Game game = new Game();
        private HttpClient client;

        private InputEnum lastInput;
        private int prevXPos;
        private int prevYPos;
        private EntityEnum prevEntity;
        private Stopwatch stopwatch = new Stopwatch();
        private IList<long> updateTimeList = new List<long>();

        public Form1()
        {
            setUpImages();

            client = new HttpClient();

            loginForm();
            game.ResetMap();

            initPlayers();

            InitializeComponent();

            initRectangles();

            putInfo(serverURLInput + gameKey, JsonConvert.SerializeObject(new Input(playerKey, InputEnum.None)));
            Thread.Sleep(500);

            RT(() => {
                updateMap();
            }, 10);
        }
        
        private void setUpImages()
        {
            float fw = rectWidth / 100f;
            float fh = rectHeight / 100f;

            WallBrush.ScaleTransform(fw, fh, System.Drawing.Drawing2D.MatrixOrder.Prepend);
            BreakableWallBrush.ScaleTransform(fw, fh, System.Drawing.Drawing2D.MatrixOrder.Prepend);
            BombBrush.ScaleTransform(fw, fh, System.Drawing.Drawing2D.MatrixOrder.Prepend);
            EmptyBrush.ScaleTransform(fw, fh, System.Drawing.Drawing2D.MatrixOrder.Prepend);
            ExplosionBrush.ScaleTransform(fw, fh, System.Drawing.Drawing2D.MatrixOrder.Prepend);
            ErrorBrush.ScaleTransform(fw, fh, System.Drawing.Drawing2D.MatrixOrder.Prepend);

            foreach (TextureBrush b in PlayerBrush)
            {
                b.ScaleTransform(fw, fh, System.Drawing.Drawing2D.MatrixOrder.Prepend);
            }
        }

        protected void exitGame()
        {
            putInfo(serverURLRemovePlayer + gameKey, JsonConvert.SerializeObject(new Input(playerKey, InputEnum.None)));
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
                resetGameOnServer();
            } else if (d == DialogResult.Cancel)
            {
                System.Windows.Forms.Application.Exit();
            } else if (d == DialogResult.Yes)
            {
                playerKey = value1;
                gameKey = value2;
            }
        }

        private void updateMap()
        {
            getGameStateFromServer();
            Invalidate();
        }

        private void calculateTimeUntilInputProccessed()
        {
            switch (lastInput)
            {
                case InputEnum.Up:
                    if (game.GetPlayer(playerKey).YPosition == prevYPos - 1)
                    {
                        saveElapsedTimeUntilUpdate();
                    }
                    break;
                case InputEnum.Down:
                    if (game.GetPlayer(playerKey).YPosition == prevYPos + 1)
                    {
                        saveElapsedTimeUntilUpdate();
                    }
                    break;
                case InputEnum.Left:
                    if (game.GetPlayer(playerKey).XPosition == prevXPos - 1)
                    {
                        saveElapsedTimeUntilUpdate();
                    }
                    break;
                case InputEnum.Right:
                    if (game.GetPlayer(playerKey).XPosition == prevXPos + 1)
                    {
                        saveElapsedTimeUntilUpdate();
                    }
                    break;
                case InputEnum.Bomb:
                    if (prevEntity != EntityEnum.Bomb && game.Map[prevYPos][prevXPos].EntityType == EntityEnum.Bomb)
                    {
                        saveElapsedTimeUntilUpdate();
                    }
                    break;
            }
        }

        private void saveElapsedTimeUntilUpdate()
        {
            stopwatch.Stop();
            updateTimeList.Add(stopwatch.ElapsedMilliseconds);
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

            if(game != null) setPrevValues();

            game = JsonConvert.DeserializeObject<Game>(jgame);

            if(game != null) calculateTimeUntilInputProccessed();
        }

        private void setPrevValues()
        {
            Player currentPlayer = game.GetPlayer(playerKey);
            prevXPos = currentPlayer.XPosition;
            prevYPos = currentPlayer.YPosition;
            prevEntity = game.Map[prevYPos][prevXPos].EntityType;
        }

        private void communicate(InputEnum inp)
        {
            putInfo(serverURLInput + gameKey, JsonConvert.SerializeObject(new Input(playerKey, inp)));
            stopwatch.Restart();
            lastInput = inp;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            drawRectanglesMap(g);

            drawRectanglesPlayer(g);

            try
            {
                updateDelayText.Text = updateTimeList[updateTimeList.Count - 1].ToString() + " ms";
            } catch (Exception)
            {
                updateDelayText.Text = "0 ms";
            }
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
                        //g.DrawRectangle(Pens.Black, rectPlayer[i]);
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
                    //g.DrawRectangle(Pens.Black, rectMap[j, i]);
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
            Button buttonReset = new Button();

            form.Text = title;
            label.Text = promptText;
            textBoxUsername.Text = valueUsername;
            textBoxServer.Text = valueServer;

            buttonOk.Text = "Join";
            buttonReset.Text = "New Game";
            buttonOk.DialogResult = DialogResult.OK;
            buttonReset.DialogResult = DialogResult.Yes;

            label.SetBounds(9, 10, 382, 13);
            textBoxUsername.SetBounds(12, 36, 372, 20);
            textBoxServer.SetBounds(12, 66, 372, 20);
            buttonOk.SetBounds(173, 97, 75, 30);
            buttonReset.SetBounds(254, 97, 75, 30);

            label.AutoSize = true;
            textBoxUsername.Anchor = textBoxUsername.Anchor | AnchorStyles.Right;
            textBoxServer.Anchor = textBoxServer.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonReset.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new System.Drawing.Size(396, 130);
            form.Controls.AddRange(new Control[] { label, textBoxUsername, buttonOk, buttonReset });
            form.Controls.AddRange(new Control[] { label, textBoxServer, buttonOk, buttonReset });
            form.ClientSize = new System.Drawing.Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonReset;

            DialogResult dialogResult = form.ShowDialog();
            valueUsername = textBoxUsername.Text;
            valueServer = textBoxServer.Text;
            return dialogResult;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
