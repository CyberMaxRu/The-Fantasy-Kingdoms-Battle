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
        private Timer timerStep;
        private int pastFrames;
        private Bitmap bmpBackground;
        private Stopwatch timePassed = new Stopwatch();
        private bool inDraw;

        private readonly Label lblPlayer1;
        private readonly Label lblPlayer2;
        private Point pointAvatarPlayer1;
        private Point pointAvatarPlayer2;
        private Rectangle rectBandHealthPlayer1;
        private Rectangle rectBandHealthPlayer2;
        private readonly SolidBrush brushHealth = new SolidBrush(Color.Green);
        private readonly SolidBrush brushNoneHealth = new SolidBrush(Color.LightGreen);
        private readonly Label lblStateBattle;
        private readonly Label lblTimer;
        private readonly Button btnEndBattle;
        private readonly Label lblDamagePlayer1;
        private readonly Label lblDamagePlayer2;
        private int pastSeconds;

        private int maxHealthPlayer1;
        private int maxHealthPlayer2;

        public FormBattle()
        {
            InitializeComponent();

            Paint += FormBattle_Paint;
            FormClosing += FormBattle_FormClosing;

            penArrow.Width = 3;
            penArrow.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4.0F, 8.0F, true);

            // Создаем контролы
            lblPlayer1 = new Label()
            {
                Parent = this,
                Top = Config.GRID_SIZE,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Width = Program.formMain.ilPlayerAvatarsBig.ImageSize.Width,
                Height = 24,
                TextAlign = ContentAlignment.BottomCenter,
                Font = new Font("Times New Roman", 12, FontStyle.Bold),                
            };

            lblPlayer2 = new Label()
            {
                Parent = this,
                Top = Config.GRID_SIZE,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Width = Program.formMain.ilPlayerAvatarsBig.ImageSize.Width,
                Height = 24,
                TextAlign = ContentAlignment.BottomCenter,
                Font = new Font("Times New Roman", 12, FontStyle.Bold)
            };

            lblStateBattle = new Label()
            {
                Parent = this,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Calibri", 20, FontStyle.Bold)
            };

            lblTimer = new Label()
            {
                Parent = this,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Calibri", 20, FontStyle.Bold)
            };

            lblDamagePlayer1 = new Label()
            {
                Parent = this,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                ImageList = Program.formMain.ilGui24,
                ImageIndex = FormMain.GUI_24_STAR,
                Height = 24,
                Width = 64,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold)
            };

            lblDamagePlayer2 = new Label()
            {
                Parent = this,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                ImageList = Program.formMain.ilGui24,
                ImageIndex = FormMain.GUI_24_STAR,
                Height = 24,
                Width = 64,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold)
            };

            btnEndBattle = new Button()
            {
                Parent = this,
                Text = "Завершить бой"
            };
            btnEndBattle.Click += BtnEndBattle_Click;

            // Таймер для анимации
            timerStep = new Timer()
            {
                Interval = 1000 / Config.STEPS_IN_SECOND,
                Enabled = false
            };
            timerStep.Tick += TimerStep_Tick;
        }

        private void BtnEndBattle_Click(object sender, EventArgs e)
        {
            timerStep.Stop();

            battle.CalcWholeBattle();

            ApplyStep();
        }

        private void TimerStep_Tick(object sender, EventArgs e)
        {
            if (inDraw == false)
            {
                inDraw = true;

                if (battle.BattleCalced == false)
                {
                    // Рисуем столько кадров, сколько должно было пройти
                    pastFrames = (int)(timePassed.ElapsedMilliseconds / Config.STEP_IN_MSEC);
                    while ((battle.Step <= pastFrames) && (battle.BattleCalced == false))
                        battle.CalcStep();

                    DoFrame();
                }

                inDraw = false;
            }
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
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImageUnscaled(bmpBackground, 0, 0);
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            Bitmap bmpPanel = new Bitmap(cellHeroes[0, 0].Width, cellHeroes[0, 0].Height);

            // Рисуем аватарки игроков
            e.Graphics.DrawImageUnscaled(Program.formMain.ilPlayerAvatarsBig.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatarsBig, battle.Player1.ImageIndexAvatar, (battle.BattleCalced == false) || (battle.Winner == battle.Player1))], pointAvatarPlayer1);
            e.Graphics.DrawImageUnscaled(Program.formMain.ilPlayerAvatarsBig.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatarsBig, battle.Player2.ImageIndexAvatar, (battle.BattleCalced == false) || (battle.Winner == battle.Player2))], pointAvatarPlayer2);

            // Рисуем полоски жизней героев игроков
            GuiUtils.DrawBand(e.Graphics, rectBandHealthPlayer1, brushHealth, brushNoneHealth, CalcHealthPlayer(battle.Player1), maxHealthPlayer1);
            GuiUtils.DrawBand(e.Graphics, rectBandHealthPlayer2, brushHealth, brushNoneHealth, CalcHealthPlayer(battle.Player2), maxHealthPlayer2);

            // Рисуем героев
            for (int y = 0; y < battle.SizeBattlefield.Height; y++)
                for (int x = 0; x < battle.SizeBattlefield.Width; x++)
                {
                    if (cellHeroes[y, x].Hero != null)
                    {
                        cellHeroes[y, x].DrawToBitmap(bmpPanel, new Rectangle(0, 0, bmpPanel.Width, bmpPanel.Height));
                        e.Graphics.DrawImageUnscaled(bmpPanel, cellHeroes[y, x].Left, cellHeroes[y, x].Top);
                    }
                }

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

            // 
            if (battle.BattleCalced == false)
                lblStateBattle.Text = "Идет бой";
            else
            {
                if ((battle.Winner != null) && (battle.Winner.TypePlayer == TypePlayer.Human))
                {
                    lblStateBattle.Text = "Победа!";
                    lblStateBattle.ForeColor = Color.Green;
                }
                else if (battle.Winner == null)
                {
                    lblStateBattle.Text = "Ничья";
                    lblStateBattle.ForeColor = Color.White;
                }
                else
                {
                    lblStateBattle.Text = "Поражение";
                    lblStateBattle.ForeColor = Color.Red;
                }
            }

            pastSeconds = battle.Step / Config.STEPS_IN_SECOND;
            TimeSpan ts = new TimeSpan(0, 0, pastSeconds);
            lblTimer.Text = ts.ToString("mm':'ss");
        }

        internal void ShowBattle(Battle b)
        {
            Text = "Бой. " + b.Player1.Name + " vs " + b.Player2.Name;

            battle = b;
            PanelHeroInBattle p;

            // Расчет координат для отрисовки
            PanelHeroInBattle phb = new PanelHeroInBattle(Program.formMain.ilGuiHeroes);

            Width = Config.GRID_SIZE + (phb.Width + Config.GRID_SIZE) * Config.HERO_ROWS * 2 + Config.GRID_SIZE + (Width - ClientSize.Width);

            lblPlayer1.Left = Config.GRID_SIZE;
            lblPlayer1.Text = battle.Player1.Name;
            pointAvatarPlayer1 = new Point(lblPlayer1.Left, lblPlayer1.Top + lblPlayer1.Height);
            pointAvatarPlayer2 = new Point(ClientSize.Width - Program.formMain.ilPlayerAvatarsBig.ImageSize.Width - Config.GRID_SIZE, pointAvatarPlayer1.Y);
            lblPlayer2.Left = pointAvatarPlayer2.X;
            lblPlayer2.Text = battle.Player2.Name;

            rectBandHealthPlayer1 = new Rectangle(pointAvatarPlayer1.X, pointAvatarPlayer1.Y + Program.formMain.ilPlayerAvatarsBig.ImageSize.Height + 2, Program.formMain.ilPlayerAvatarsBig.ImageSize.Width, 6);
            rectBandHealthPlayer2 = new Rectangle(pointAvatarPlayer2.X, pointAvatarPlayer2.Y + Program.formMain.ilPlayerAvatarsBig.ImageSize.Height + 2, Program.formMain.ilPlayerAvatarsBig.ImageSize.Width, 6);

            // Считаем максимальное количество здоровья у героев игроков
            maxHealthPlayer1 = CalcHealthPlayer(b.Player1);
            maxHealthPlayer2 = CalcHealthPlayer(b.Player2);

            //
            lblStateBattle.Top = pointAvatarPlayer1.Y;
            lblStateBattle.Width = pointAvatarPlayer2.X - pointAvatarPlayer1.X - Program.formMain.ilPlayerAvatarsBig.ImageSize.Width - Config.GRID_SIZE * 2;
            lblStateBattle.Left = pointAvatarPlayer1.X + Program.formMain.ilPlayerAvatarsBig.ImageSize.Width + Config.GRID_SIZE;

            lblTimer.Top = lblStateBattle.Top + lblStateBattle.Height;
            lblTimer.Width = lblStateBattle.Width;
            lblTimer.Left = lblStateBattle.Left;

            btnEndBattle.Top = lblTimer.Top + lblTimer.Height + Config.GRID_SIZE;
            btnEndBattle.Width = 120;
            btnEndBattle.Height = 32;
            btnEndBattle.Left = pointAvatarPlayer1.X + Program.formMain.ilPlayerAvatarsBig.ImageSize.Width + (((pointAvatarPlayer2.X - pointAvatarPlayer1.X - Program.formMain.ilPlayerAvatarsBig.ImageSize.Width) - btnEndBattle.Width) / 2);

            //
            lblDamagePlayer1.Top = btnEndBattle.Top;
            lblDamagePlayer1.Left = pointAvatarPlayer1.X + Program.formMain.ilPlayerAvatarsBig.ImageSize.Width + Config.GRID_SIZE;
            lblDamagePlayer1.Hide();
            lblDamagePlayer2.Top = btnEndBattle.Top;
            lblDamagePlayer2.Left = pointAvatarPlayer2.X - Config.GRID_SIZE - 80;
            lblDamagePlayer2.Hide();
            //
            int topCells = rectBandHealthPlayer1.Y + rectBandHealthPlayer1.Height + Config.GRID_SIZE;
            cellHeroes = new PanelHeroInBattle[b.SizeBattlefield.Height, b.SizeBattlefield.Width];
            for (int y = 0; y < b.SizeBattlefield.Height; y++)
                for (int x = 0; x < b.SizeBattlefield.Width; x++)
                {
                    p = new PanelHeroInBattle(Program.formMain.ilGuiHeroes)
                    {
                        //Parent = this,
                        Left = Config.GRID_SIZE + (x * (phb.Width + Config.GRID_SIZE)),
                        Top = topCells + (y * (phb.Height + Config.GRID_SIZE))
                    };

                    cellHeroes[y, x] = p;
                }

            Height = topCells + (b.SizeBattlefield.Height * (phb.Height + Config.GRID_SIZE)) + (Height - ClientSize.Height);

            // Подготавливаем подложку
            bmpBackground = GuiUtils.MakeBackground(ClientSize);

            //
            ApplyStep();

            timerStep.Start();
            timePassed.Start();
            inDraw = false;

            phb.Dispose();

            ShowDialog();
        }

        private int CalcHealthPlayer(Player p)
        {
            int health = 0;
            foreach (HeroInBattle h in battle.ActiveHeroes)
            {
                if (h.Player == p)
                    health += h.CurrentHealth;
            }

            return health;
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

            btnEndBattle.Enabled = !battle.BattleCalced;
            if (battle.BattleCalced)
            {
                if (battle.Winner == battle.Player1)
                {
                    lblDamagePlayer1.Show();
                    lblDamagePlayer1.Text = battle.Player1.LastBattleDamageToCastle.ToString();
                }
                else if (battle.Winner == battle.Player2)
                {
                    lblDamagePlayer2.Show();
                    lblDamagePlayer2.Text = battle.Player2.LastBattleDamageToCastle.ToString();
                }
            }

            Refresh();
        }
    }
}
