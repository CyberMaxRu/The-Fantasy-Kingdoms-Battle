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

namespace Fantasy_King_s_Battle
{
    public partial class FormBattle : Form
    {
        private PanelHeroInBattle[,] cellHeroes;
        private Battle battle;
        private Pen penArrow = new Pen(Color.Fuchsia);
        private Bitmap bmpBackground;
        private Timer timerStep;
        private DateTime lastLabel;
        private int frames;
        private int ticksPast = 0;
        private DateTime startDateTime;
        private Bitmap background;
        private Stopwatch st = new Stopwatch();
        private Stopwatch timePassed = new Stopwatch();
        private bool inDraw;
        private int skippedFrames = 0;

        public FormBattle()
        {
            InitializeComponent();

            Paint += FormBattle_Paint;
            FormClosing += FormBattle_FormClosing;

            //BackgroundImage = Program.formMain.background;
            penArrow.Width = 3;
            penArrow.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4.0F, 8.0F, true);
            //bmpBackground = new Bitmap(Width, Height);
            //bmpBackground.

            // Подготавливаем подложку
            background = new Bitmap(ClientSize.Width, ClientSize.Height);
            int repX = ClientSize.Width / Program.formMain.background.Width + 1;
            int repY = ClientSize.Height / Program.formMain.background.Height  + 1; 
            Graphics g = Graphics.FromImage(background);
            for (int y = 0; y < repY; y++)
                for (int x = 0; x < repX; x++)
                    g.DrawImageUnscaled(Program.formMain.background, x * Program.formMain.background.Width, y * Program.formMain.background.Height);
            g.Dispose();

            // Таймер для анимации
            timerStep = new Timer()
            {
                Interval = 1000 / Config.STEPS_IN_SECOND,
                Enabled = false
            };
            timerStep.Tick += TimerStep_Tick;
        }

        private void TimerStep_Tick(object sender, EventArgs e)
        {
            if (ticksPast == 0)
            {
                startDateTime = DateTime.Now;
                ticksPast = 1;
            }

            if (inDraw == false)
            {
                inDraw = true;
                frames++;

                if ((DateTime.Now - lastLabel).TotalMilliseconds > 1000)
                {
                    lblSpeed.Text = frames.ToString();
                    lastLabel = DateTime.Now;
                    frames = 0;
                }

                if (battle.BattleCalced == false)
                {
                    // Рисуем столько кадров, сколько должно было пройти
                    int pastFrames = (int)(timePassed.ElapsedMilliseconds / Config.STEP_IN_MSEC);
                    while (battle.Step <= pastFrames)
                        battle.CalcStep();

                    DoFrame();
                }

                inDraw = false;
            }
            else
                skippedFrames++;
        }

        private void DoFrame()
        {
            ApplyStep();
            Application.DoEvents();
        }

        private void FormBattle_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (battle.BattleCalced == false)
                battle.CalcWholeBattle(); 
        }

        private void FormBattle_Paint(object sender, PaintEventArgs e)
        {
            st.Restart();
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImageUnscaled(background, 0, 0);
            st.Stop();
            lblDrawBack.Text = st.ElapsedMilliseconds.ToString();

            Bitmap bmpPanel = new Bitmap(cellHeroes[0, 0].Width, cellHeroes[0, 0].Height);

            // Рисуем героев
            st.Restart();
            for (int y = 0; y < battle.SizeBattlefield.Height; y++)
                for (int x = 0; x < battle.SizeBattlefield.Width; x++)
                {
                    if (cellHeroes[y, x].Hero != null)
                    {
                        cellHeroes[y, x].DrawToBitmap(bmpPanel, new Rectangle(0, 0, bmpPanel.Width, bmpPanel.Height));
                        e.Graphics.DrawImageUnscaled(bmpPanel, cellHeroes[y, x].Left, cellHeroes[y, x].Top);
                    }
                }
            st.Stop();
            lblDrawHeroes.Text = st.ElapsedMilliseconds.ToString();

            foreach (HeroInBattle h in battle.ActiveHeroes)
            {
                if (h.Target != null)
                {
                    PanelHeroInBattle p1 = cellHeroes[h.Coord.Y, h.Coord.X];
                    PanelHeroInBattle p2 = cellHeroes[h.Target.Coord.Y, h.Target.Coord.X];

                    penArrow.Color = h.PlayerHero.Player == battle.Player1 ? Color.Green : Color.Maroon;
                    e.Graphics.DrawLine(penArrow, new Point(p1.Location.X + p1.Width / 2 , p1.Location.Y + p1.Height / 2), new Point(p2.Location.X + p2.Width / 2, p2.Location.Y + p2.Height / 2));
                }
            }
        }

        internal void ShowBattle(Battle b)
        {
            battle = b;
            PanelHeroInBattle p;

            cellHeroes = new PanelHeroInBattle[b.SizeBattlefield.Height, b.SizeBattlefield.Width];
            for (int y = 0; y < b.SizeBattlefield.Height; y++)
                for (int x = 0; x < b.SizeBattlefield.Width; x++)
                {
                    p = new PanelHeroInBattle(Program.formMain.ilGuiHeroes)
                    {
                        //Parent = this,
                        Left = Config.GRID_SIZE + (x * (Program.formMain.ilGuiHeroes.ImageSize.Width + Config.GRID_SIZE)),
                        Top = Config.GRID_SIZE * 4 + (y * (Program.formMain.ilGuiHeroes.ImageSize.Height + 16 + Config.GRID_SIZE))
                    };

                    cellHeroes[y, x] = p;
                }

            ApplyStep();
            timerStep.Start();
            lastLabel = DateTime.Now;
            frames = 0;
            inDraw = false;
            timePassed.Start();
            ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (battle.BattleCalced == false)
            {
                battle.CalcStep();
                ApplyStep();
            }
        }

        private void ApplyStep()
        {
            for (int y = 0; y < battle.SizeBattlefield.Height; y++)
                for (int x = 0; x < battle.SizeBattlefield.Width; x++)
                    cellHeroes[y, x].Hero = null;

            foreach (HeroInBattle h in battle.ActiveHeroes)
            {
                Debug.Assert(cellHeroes[h.Coord.Y, h.Coord.X].Hero == null);

                cellHeroes[h.Coord.Y, h.Coord.X].Hero = h;
            }

            lblStep.Text = "Шаг: " + battle.Step.ToString() + " / " + ((DateTime.Now - startDateTime).TotalMilliseconds / (1000 / Config.STEPS_IN_SECOND)).ToString();
            lblTotalSteps.Text = battle.BattleCalced == false ? "Идет бой" : "Бой закончен";
            lblSkippedFrames.Text = skippedFrames.ToString();

            Refresh();
        }
    }
}
