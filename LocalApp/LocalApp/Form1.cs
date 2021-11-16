using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;



namespace ServerlessBombermanLocal
{
    public partial class Form1 : Form
    {
        private int[,] map = new int [,]{ { 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 0, 0, 0, 0, 0, 0, 1 }, { 1, 0, 2, 2, 2, 2, 0, 1 }, { 1, 0, 2, 0, 0, 2, 0, 1 }, { 1, 0, 2, 0, 0, 2, 0, 1 }, { 1, 0, 2, 2, 2, 2, 0, 1 }, { 1, 0, 0, 0, 0, 0, 0, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1 } };
        private int dx, dy, w, h;
        private Rectangle rect;
        private Rectangle[,] rectMap;
        private int mapSize = 8;
        private int rectWidth = 97;
        private int rectHeight = 97;
        private static String serverURL = "https://serverlessbomberman.azurewebsites.net/api/";
        private int[] playerPosition = new int[] {1, 1};
        
        public class DataFormatReceive
        {
            public int[,] map { get; set; }
            public int[] playerPosition { get; set; }

            public DataFormatReceive(int[,] tMap, int[] tPlayerPosition)
            {
                map = tMap;
                playerPosition = tPlayerPosition;
            }
        }
        public class DataFormatSend
        {
            public int[,] map { get; set; }
            public int move { get; set; }

            public DataFormatSend(int[,] tMap, int tMove)
            {
                map = tMap;
                move = tMove;
            }
        }

        public Form1()
        {
            dx = rectWidth;
            dy = rectHeight;
            w = rectWidth;
            h = rectHeight;
            InitializeComponent();
            rectMap = new Rectangle[mapSize, mapSize];

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    rectMap[i, j] = new Rectangle(i*rectWidth, j*rectHeight, rectWidth, rectHeight);
                }
            }
            rect = new Rectangle(playerPosition[0], playerPosition[1], w, h);
        }

        private async void getInfo(String id)
        {
            int n = 1;
            var client = new HttpClient();

            var tasks = new List<Task<string>>();

            for (int i = 0; i < n; i++)
            {
                async Task<string> func()
                {
                    var response = await client.GetAsync(serverURL+ "getgamestate/" + id);
                    return await response.Content.ReadAsStringAsync();
                }

                tasks.Add(func());
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);

            var getResponses = new List<string>();
            foreach (var t in tasks)
            {
                string postResponse = await t; //t.Result would be okay too.
                getResponses.Add(postResponse);
            }

            String[] ss = getResponses.ToArray();
            Console.WriteLine(ss.Length);
            for (int i = 0; i < ss.Length; i++)
            {
                Console.WriteLine(ss[i]);
                DataFormatReceive recv = JsonSerializer.Deserialize<DataFormatReceive>(ss[i]);
                map = recv.map;
                playerPosition = recv.playerPosition;

            }

        }

        private async void putInfo(String id, String message)
        {
            var n = 1;
            var client = new HttpClient();

            var tasks = new List<Task<string>>();
            for (int i = 0; i < n; i++)
            {
                async Task<string> func()
                {
                    var response = await client.PostAsync(serverURL + "input/" + id, new StringContent(message));
                    return await response.Content.ReadAsStringAsync();
                }

                tasks.Add(func());
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async void communicate(int move)
        {
            String gameID = "gameId0";
            putInfo(gameID, move.ToString());
            getInfo(gameID);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            /*
            using (var pen = new Pen(Color.Red))
            using (var brush = new HatchBrush(HatchStyle.Percent10, Color.Blue, Color.Transparent))
            {
                e.Graphics.FillRectangle(brush, rect);
                e.Graphics.DrawRectangle(pen, rect);
            }
            */
            Graphics g = e.Graphics;
            SolidBrush playerBrush = new SolidBrush(Color.Blue);
            SolidBrush WallBrush = new SolidBrush(Color.Black);
            SolidBrush BreakableWallBrush = new SolidBrush(Color.Gray);
            SolidBrush EmptyBrush = new SolidBrush(Color.White);

            rect = new Rectangle(playerPosition[0] * rectWidth, playerPosition[1] * rectHeight, w, h);

            //g.FillRectangle(hb1, rect);
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    switch (map[i,j]) {
                        case 0:
                            g.FillRectangle(EmptyBrush, rectMap[i, j]);
                            break;
                        case 1:
                            g.FillRectangle(WallBrush, rectMap[i, j]);
                            break;
                        case 2:
                            g.FillRectangle(BreakableWallBrush, rectMap[i, j]);
                            break;
                    }
                    g.DrawRectangle(Pens.Black, rectMap[i, j]);
                }
            }

            // draw player
            g.DrawRectangle(Pens.Green, rect);
            g.FillRectangle(playerBrush, rect);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled) {
                if (e.KeyCode == Keys.Up)
                {
                    //rect.Offset(0, -dy);
                    communicate(0);
                    //BackColor = Color.FromName("Blue");
                    //MessageBox.Show("You pressed the Up key.");
                }
                else if (e.KeyCode == Keys.Down)
                {
                    //rect.Offset(0, dy);
                    communicate(1);
                    //BackColor = Color.FromName("Red");
                    //MessageBox.Show("You pressed the Down key.");
                } else if (e.KeyCode == Keys.Right)
                {
                    //rect.Offset(dx, 0);
                    communicate(2);
                    //BackColor = Color.FromName("Green");
                    //MessageBox.Show("You pressed the Right key.");
                }
                else if (e.KeyCode == Keys.Left)
                {
                    //rect.Offset(-dx, 0);
                    communicate(3);
                    //BackColor = Color.FromName("Yellow");
                    //MessageBox.Show("You pressed the Left key.");
                }
                Invalidate();
                e.Handled = true;
            }
        }
    }
}
