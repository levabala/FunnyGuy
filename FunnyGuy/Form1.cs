using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace FunnyGuy
{
    public partial class Form1 : Form
    {
        List<Fruit> fruits = new List<Fruit>()
        {
            new Fruit(5, 3, 1),
            new Fruit(4, 6, 1),
        };
        Guy guy = new Guy(2, 2, 1, 2, 2);

        float scale = 20;
        float lastTime = 0;
        int maxFruitsCount = 10;
        static int score = 0;
        Stopwatch stopWatch = new Stopwatch();
        Timer renderTimer = new Timer();

        static float changeDelta = 0.6f;
        static float maxDelta = 9;
        GameAction? action1, action2;

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;

            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
        }

        Dictionary<Keys, GameAction> key2actionMap = new Dictionary<Keys, GameAction>()
            {
                { Keys.Right, GameAction.Right},
                { Keys.Up, GameAction.Top},
                { Keys.Down, GameAction.Bottom},
                { Keys.Left, GameAction.Left},
            };
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (key2actionMap.ContainsKey(e.KeyCode))
            {
                GameAction action = key2actionMap[e.KeyCode];

                if (action1 == action)
                    action1 = null;
                if (action2 == action)
                    action2 = null;
            }                
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {            
            if (key2actionMap.ContainsKey(e.KeyCode))
                if (action1 == null && action2 != key2actionMap[e.KeyCode])
                    action1 = key2actionMap[e.KeyCode];
                else if (action2 == null && action1 != key2actionMap[e.KeyCode])
                    action2 = key2actionMap[e.KeyCode];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Paint += Form1_Paint;

            renderTimer.Interval = 4;
            renderTimer.Tick += (s, a) =>
            {
                tick();
                Invalidate();
            };
            renderTimer.Start();

            stopWatch.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            render(e.Graphics);
        }

        Dictionary<GameAction, Action<Guy>> gameAction2ActionMap = new Dictionary<GameAction, Action<Guy>>()
            {
                { GameAction.Right, (Guy guy) => { guy.sx = Math.Min(guy.sx + changeDelta, maxDelta); } },
                { GameAction.Left, (Guy guy) => { guy.sx = Math.Max(guy.sx - changeDelta, -maxDelta); } },
                { GameAction.Bottom, (Guy guy) => { guy.sy = Math.Min(guy.sy + changeDelta, maxDelta); } },
                { GameAction.Top, (Guy guy) => { guy.sy = Math.Max(guy.sy - changeDelta, -maxDelta); } },
            };

        private void tick()
        {
            float elaspesSeconds = stopWatch.ElapsedMilliseconds / 1000f;
            float timeDelta = elaspesSeconds - lastTime;

            if (action1 != null)
                gameAction2ActionMap[action1.GetValueOrDefault()](guy);
            if (action2 != null)
                gameAction2ActionMap[action2.GetValueOrDefault()](guy);

            if (fruits.Count < maxFruitsCount)
                spawnFruit();
            
            guy.move(timeDelta);

            fruits = fruits.Where(f =>
            {
                bool nyam = checkNyam(f);
                if (nyam)
                    score++;

                return !nyam;
            }).ToList();

            lastTime = elaspesSeconds;

            Text = string.Format("Score: {0}", score);
        }

        private bool checkNyam(Fruit fruit)
        {
            float dx = fruit.x - guy.x;
            float dy = fruit.y - guy.y;

            return Math.Sqrt(dx * dx + dy * dy) < (fruit.size + guy.size) / 2;
        }

        private void spawnFruit()
        {
            Random rnd = new Random();
            float x = rnd.Next(0, (int)(ClientSize.Width / scale));
            float y = rnd.Next(0, (int)(ClientSize.Height / scale));
            float size = (float)rnd.NextDouble() * 5;

            Fruit f = new Fruit(x, y, size);

            fruits.Add(f);
        }

        private void render(Graphics g)
        {
            g.FillEllipse(
                Brushes.DarkGreen, 
                (guy.x - guy.size / 2) * scale,
                (guy.y - guy.size / 2) * scale,
                guy.size * scale, 
                guy.size * scale
            );

            fruits.ForEach(f =>
                g.FillEllipse(
                    Brushes.DarkRed,
                    (f.x - f.size / 2) * scale,
                    (f.y - f.size / 2) * scale,
                    f.size * scale,
                    f.size * scale
                )
            );
        }
    }

    enum GameAction
    {
        None,
        Top,
        Right,
        Left,
        Bottom
    }
}
