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
        enum SpeedBattle { VerySlow, Slow, Normal, Fast, VeryFast };

        private PanelHeroInBattle[,] cellHeroes;
        private Battle battle;
        private Pen penArrow = new Pen(Color.Fuchsia);
        private Pen penCircle = new Pen(new SolidBrush(Color.Fuchsia));
        private Bitmap bmpBackground;
        private Pen penGrid = new Pen(Color.Gray);
        private Size sizeTile;
        private Size sizeCell;
        private Point topLeftGrid;
        private Point topLeftCells;
        private const int WIDTH_LINE = 1;

        // Время битвы
        private SpeedBattle currentSpeed = SpeedBattle.Normal;
        private bool inPause = false;
        private readonly List<DateTime> Frames = new List<DateTime>();
        private readonly List<DateTime> Steps = new List<DateTime>();
        private readonly List<DateTime> Paints = new List<DateTime>();
        private DateTime timeStart;// Время начала битвы
        private DateTime timeInternalFixed;// Внутреннее время битвы, с учетом изменения скорости
        private DateTime timeInternalApprox;// Ориентировочное время битвы
        private int stepsCalcedByCurrentSpeed;// Сколько шагов посчитано по текущей скорости
        private Stopwatch timePassedCurrentSpeed = new Stopwatch();// Время, прошедшее с начала задействования текущей скорости

        private bool needClose = false;

        private readonly Label lblSystemInfo;
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
        private readonly Button btnPlayPause;
        private readonly Button btnDecSpeed;
        private readonly Button btnIncSpeed;
        private readonly Label lblDamagePlayer1;
        private readonly Label lblDamagePlayer2;

        private int maxHealthPlayer1;
        private int maxHealthPlayer2;

        public FormBattle()
        {
            InitializeComponent();

            Paint += FormBattle_Paint;
            FormClosing += FormBattle_FormClosing;

            penArrow.Width = 2;
            penArrow.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(3.0F, 6.0F, true);

            // Создаем контролы
            lblSystemInfo = new Label()
            {
                Parent = this,
                Top = FormMain.Config.GridSize,
                Width = 320,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Height = 24,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Times New Roman", 12, FontStyle.Bold),
            };

            lblPlayer1 = new Label()
            {
                Parent = this,
                Top = GuiUtils.NextTop(lblSystemInfo),
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
                Top = lblPlayer1.Top,
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
                Height = 32,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Calibri", 20, FontStyle.Bold)
            };

            lblTimer = new Label()
            {
                Parent = this,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Height = 32,
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

            btnPlayPause = new Button()
            {
                Parent = this,
                Text = "Скорость"
            };
            btnPlayPause.Click += BtnPlayPause_Click;

            btnDecSpeed = new Button()
            {
                Parent = this,
                Text = "<",
                Width = FormMain.Config.GridSize * 3
            };
            btnDecSpeed.Click += BtnDecSpeed_Click;

            btnIncSpeed = new Button()
            {
                Parent = this,
                Text = ">",
                Width = FormMain.Config.GridSize * 3
            };
            btnIncSpeed.Click += BtnIncSpeed_Click;

            ApplySpeed();
        }

        // Фиксация прошедшего внутреннего времени битвы с текущей скоростью
        private void LockPassedTimeSpeed()
        {
            timeInternalFixed = timeInternalFixed.AddMilliseconds(timePassedCurrentSpeed.ElapsedMilliseconds * ValueSpeed());
            timePassedCurrentSpeed.Reset();
            timePassedCurrentSpeed.Start();
            stepsCalcedByCurrentSpeed = 0;
        }

        private void BtnIncSpeed_Click(object sender, EventArgs e)
        {
            Debug.Assert(!battle.BattleCalced);

            // Фиксируем итоговое количество времени с текущей скоростью
            LockPassedTimeSpeed();

            switch (currentSpeed)
            {
                case SpeedBattle.VerySlow:
                    currentSpeed = SpeedBattle.Slow;
                    break;
                case SpeedBattle.Slow:
                    currentSpeed = SpeedBattle.Normal;
                    break;
                case SpeedBattle.Normal:
                    currentSpeed = SpeedBattle.Fast;
                    break;
                case SpeedBattle.Fast:
                    currentSpeed = SpeedBattle.VeryFast;
                    break;
                default:
                    break;
            }

            ApplySpeed();
        }

        private void BtnDecSpeed_Click(object sender, EventArgs e)
        {
            Debug.Assert(!battle.BattleCalced);

            // Фиксируем итоговое количество времени с текущей скоростью
            LockPassedTimeSpeed();

            switch (currentSpeed)
            {
                case SpeedBattle.Slow:
                    currentSpeed = SpeedBattle.VerySlow;
                    break;
                case SpeedBattle.Normal:
                    currentSpeed = SpeedBattle.Slow;
                    break;
                case SpeedBattle.Fast:
                    currentSpeed = SpeedBattle.Normal;
                    break;
                case SpeedBattle.VeryFast:
                    currentSpeed = SpeedBattle.Fast;
                    break;
                default:
                    break;
            }

            ApplySpeed();
        }

        private void BtnPlayPause_Click(object sender, EventArgs e)
        {
            Debug.Assert(!battle.BattleCalced);

            inPause = !inPause;
            if (inPause)
            {
                Debug.Assert(timePassedCurrentSpeed.IsRunning);

                timePassedCurrentSpeed.Stop();
            }
            else
            {
                Debug.Assert(!timePassedCurrentSpeed.IsRunning);

                timePassedCurrentSpeed.Stop();
            }

            ApplySpeed();
        }

        private void ApplySpeed()
        {
            if (inPause)
            {
                btnPlayPause.Text = "Пауза";
            }
            else
            {
                btnPlayPause.Text = "Скорость " + ValueSpeed().ToString() + "x";
            }
        }

        private double ValueSpeed()
        {
            switch (currentSpeed)
            {
                case SpeedBattle.VerySlow:
                    return 0.2;
                case SpeedBattle.Slow:
                    return 0.5;
                case SpeedBattle.Normal:
                    return 1;
                case SpeedBattle.Fast:
                    return 2;
                case SpeedBattle.VeryFast:
                    return 5;
                default:
                    throw new Exception("Неизвестная скорость.");
            }
        }

        private void BtnEndBattle_Click(object sender, EventArgs e)
        {
            battle.CalcWholeBattle();

            ApplyStep();
        }

        private void TimerStep_Tick(object sender, EventArgs e)
        {
            if (inPause)
                return;

            if (battle.BattleCalced == false)
            {
                // Примерное прошедшее время битвы
                timeInternalApprox = timeInternalFixed.AddMilliseconds(timePassedCurrentSpeed.ElapsedMilliseconds * ValueSpeed());
                // Считаем, сколько шагов в секунде по текущей скорости
                int stepsPerSecond = (int)(FormMain.Config.StepsInSecond * ValueSpeed());
                // Считаем столько шагов, сколько должно было посчитано по текущей скорости
                int needSteps = (int)(timePassedCurrentSpeed.ElapsedMilliseconds / (1_000 / stepsPerSecond));

                if (stepsCalcedByCurrentSpeed < needSteps)
                {
                    while (stepsCalcedByCurrentSpeed < needSteps)
                    {
                        battle.CalcStep();
                        stepsCalcedByCurrentSpeed++;
                        Steps.Add(DateTime.Now);

                        if (battle.BattleCalced)
                            break;
                    }

                    DrawFrame();
                }
            }
        }

        private void DrawFrame()
        {
            DrawFps();
            Frames.Add(DateTime.Now);

            ApplyStep();
        }

        private void FormBattle_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (battle.BattleCalced == false)
                battle.CalcWholeBattle(); 
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImageUnscaled(bmpBackground, 0, 0);

            // Рисуем сетку
            // Вертикальные линии
            for (int x = 0; x <= battle.SizeBattlefield.Width; x++)
                e.Graphics.DrawLine(penGrid, topLeftGrid.X + x * sizeTile.Width, topLeftGrid.Y, topLeftGrid.X + x * sizeTile.Width, topLeftGrid.Y + battle.SizeBattlefield.Height * sizeTile.Height);
            // Горизонтальные линии
            for (int y = 0; y <= battle.SizeBattlefield.Height; y++)
                e.Graphics.DrawLine(penGrid, topLeftGrid.X, topLeftGrid.Y + y * sizeTile.Height, topLeftGrid.X + battle.SizeBattlefield.Width * sizeTile.Width, topLeftGrid.Y + y * sizeTile.Height);
            
            //
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            // Рисуем аватарки игроков
            e.Graphics.DrawImageUnscaled(Program.formMain.ilPlayerAvatarsBig.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatarsBig, battle.Player1.ImageIndexAvatar, (battle.BattleCalced == false) || (battle.Winner == battle.Player1))], pointAvatarPlayer1);
            e.Graphics.DrawImageUnscaled(Program.formMain.ilPlayerAvatarsBig.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatarsBig, battle.Player2.ImageIndexAvatar, (battle.BattleCalced == false) || (battle.Winner == battle.Player2))], pointAvatarPlayer2);
        }

        private void FormBattle_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            
            Bitmap bmpPanel = new Bitmap(cellHeroes[0, 0].Width, cellHeroes[0, 0].Height);
            
            // Рисуем полоски жизней героев игроков
            GuiUtils.DrawBand(e.Graphics, rectBandHealthPlayer1, brushHealth, brushNoneHealth, CalcHealthPlayer(battle.Player1), maxHealthPlayer1);
            GuiUtils.DrawBand(e.Graphics, rectBandHealthPlayer2, brushHealth, brushNoneHealth, CalcHealthPlayer(battle.Player2), maxHealthPlayer2);

            // Рисуем героев
            HeroInBattle hero;

            for (int y = 0; y < battle.SizeBattlefield.Height; y++)
                for (int x = 0; x < battle.SizeBattlefield.Width; x++)
                {
                    cellHeroes[y, x].Hero = battle.Battlefield.Tiles[y, x].Unit;
                    if (cellHeroes[y, x].Hero != null)
                    {
                        hero = cellHeroes[y, x].Hero;

                        Point shift = new Point(0, 0);
                        if (cellHeroes[y, x].Hero.TileForMove != null)
                        {
                            BattlefieldTile tileforMove = cellHeroes[y, x].Hero.TileForMove;
                            Debug.Assert(Utils.PointsIsNeighbor(cellHeroes[y, x].Hero.Coord, new Point(tileforMove.X, tileforMove.Y)) == true);
                            double percent = cellHeroes[y, x].Hero.PercentExecuteAction();

                            shift.X = (int)((cellHeroes[tileforMove.Y, tileforMove.X].Left - cellHeroes[y, x].Left) * percent);
                            shift.Y = (int)((cellHeroes[tileforMove.Y, tileforMove.X].Top - cellHeroes[y, x].Top) * percent);
                        }

                        if (((hero.Target != null) || (hero.LastTarget != default)) && (hero.PlayerHero.ClassHero.KindHero.TypeAttack == TypeAttack.Melee) && (hero.DestinationForMove == null))
                        {
                            Point coordTarget = hero.Target != null ? hero.Target.Coord : hero.LastTarget;

                            double percent = hero.PercentExecuteAction();
                            if (hero.Target == null)
                                percent = 1 - percent;

                            int shiftX = (int)((coordTarget.X > hero.Coord.X ? 1 : coordTarget.X < hero.Coord.X ? -1 : 0) * FormMain.Config.GridSize * percent);
                            int shiftY = (int)((coordTarget.Y > hero.Coord.Y ? 1 : coordTarget.Y < hero.Coord.Y ? -1 : 0) * FormMain.Config.GridSize * percent);

                            shift.X += shiftX;
                            shift.Y += shiftY;
                        }

                        Debug.Assert(cellHeroes[y, x].Left + shift.X < Width);
                        Debug.Assert(cellHeroes[y, x].Top + shift.Y >= topLeftCells.Y);
                        Debug.Assert(cellHeroes[y, x].Top + shift.Y < Height);

                        cellHeroes[y, x].DrawToBitmap(bmpPanel, new Rectangle(0, 0, bmpPanel.Width, bmpPanel.Height));
                        e.Graphics.DrawImageUnscaled(bmpPanel, cellHeroes[y, x].Left + shift.X, cellHeroes[y, x].Top + shift.Y);
                    }
                }
            
            if (battle.BattleCalced == false)
            {
                // Рисуем стрелки атаки
                foreach (HeroInBattle h in battle.ActiveHeroes)
                {
                    if ((h.Target != null) || (h.LastTarget != default) || h.DestinationForMove != null)
                    {
                        if (h.PlayerHero.ClassHero.KindHero.TypeAttack != TypeAttack.Melee)
                            if (h.Target is null)
                                continue;

                        Point coordTarget;
                        if (h.DestinationForMove == null)
                            coordTarget = h.Target != null ? h.Target.Coord : h.LastTarget;
                        else
                            coordTarget = new Point(h.DestinationForMove.X, h.DestinationForMove.Y);

                        PanelHeroInBattle p1 = cellHeroes[h.Coord.Y, h.Coord.X];
                        PanelHeroInBattle p2 = cellHeroes[coordTarget.Y, coordTarget.X];

                        // Делаем расчет точки назначения в зависимости от процент выполнения удара
                        Point pSource = new Point(p1.Location.X + p1.Width / 2, p1.Location.Y + p1.Height / 2);
                        Point pTarget = new Point(p2.Location.X + p2.Width / 2, p2.Location.Y + p2.Height / 2);

                        if (h.DestinationForMove == null)
                        {
                            double percent = h.PercentExecuteAction();
                            if (h.PlayerHero.ClassHero.KindHero.TypeAttack == TypeAttack.Melee)
                                if (h.InRollbackAction() == true)
                                    percent = 1 - percent;

                            pTarget.X = (int)(pSource.X + ((pTarget.X - pSource.X) * percent));
                            pTarget.Y = (int)(pSource.Y + ((pTarget.Y - pSource.Y) * percent));
                        }
                        else
                        {
                            pSource = new Point(topLeftGrid.X + (h.Coord.X * sizeTile.Width) + (sizeTile.Width / 2), topLeftGrid.Y + (h.Coord.Y * sizeTile.Height) + (sizeTile.Height / 2));
                            penArrow.Color = h.PlayerHero.Player == battle.Player1 ? Color.Green : Color.Maroon;

                            // Рисуем путь юнита к цели
                            foreach (BattlefieldTile t in h.PathToDestination)
                            {
                                pTarget = new Point(topLeftGrid.X + (t.Coord.X * sizeTile.Width) + (sizeTile.Width / 2), topLeftGrid.Y + (t.Coord.Y * sizeTile.Height) + (sizeTile.Height / 2) + WIDTH_LINE);
                                e.Graphics.DrawLine(penArrow, pSource, pTarget);

                                pSource = pTarget;
                            }
                        }

                        if ((h.PlayerHero.ClassHero.KindHero.TypeAttack != TypeAttack.Melee) || (h.DestinationForMove != null))
                        { 
                            penArrow.Color = h.PlayerHero.Player == battle.Player1 ? Color.Green : Color.Maroon;
                            penCircle.Color = h.PlayerHero.Player == battle.Player1 ? Color.Green : Color.Maroon;
                            if (h.PlayerHero.ClassHero.KindHero.TypeAttack == TypeAttack.Melee)
                            {
                                e.Graphics.DrawLine(penArrow, pSource, pTarget);
                            }
                            else
                            {
                                e.Graphics.DrawEllipse(penCircle, pTarget.X - 3, pTarget.Y - 3, 6, 6);
                            }
                        }
                    }
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

            int pastSeconds = battle.Step / FormMain.Config.StepsInSecond;
            TimeSpan ts = new TimeSpan(0, 0, pastSeconds);
            lblTimer.Text = ts.ToString("mm':'ss");

            bmpPanel.Dispose();

            Paints.Add(DateTime.Now);
        }

        private void DrawFps()
        {
            // Считаем Frames per second
            // Для этого удаляем все кадры, которые были более секунды назад, добавляем текущий и получаем итоговое количество
            DateTime currentDateTime = DateTime.Now;
            UpdateActions(Frames);
            UpdateActions(Steps);
            UpdateActions(Paints);

            TimeSpan realTime = currentDateTime - timeStart;
            
            lblSystemInfo.Text = Frames.Count.ToString() + " fps; steps: " + Steps.Count.ToString() + "; paints: " + Paints.Count.ToString()
                 + "; passed: " + realTime.ToString("mm':'ss"); 

            void UpdateActions(List<DateTime> list)
            {
                TimeSpan diffTime;
                for (int i = 0; i < list.Count; i++)
                {
                    diffTime = currentDateTime - list[i];
                    if (diffTime.TotalMilliseconds <= 1_000)
                    {
                        list.RemoveRange(0, i);
                        break;
                    }
                }
            }
        }

        internal void ShowBattle(Battle b)
        {
            Text = "Бой. " + b.Player1.Name + " vs " + b.Player2.Name;

            battle = b;
            PanelHeroInBattle p;

            // Расчет координат для отрисовки
            PanelHeroInBattle phb = new PanelHeroInBattle(Program.formMain.ilGuiHeroes);

            Width = FormMain.Config.GridSize + (phb.Width + FormMain.Config.GridSize) * FormMain.Config.HeroRows * 2 + FormMain.Config.GridSize + (Width - ClientSize.Width);

            lblPlayer1.Left = FormMain.Config.GridSize;
            lblPlayer1.Text = battle.Player1.Name;
            pointAvatarPlayer1 = new Point(lblPlayer1.Left, lblPlayer1.Top + lblPlayer1.Height);
            rectBandHealthPlayer1 = new Rectangle(pointAvatarPlayer1.X, pointAvatarPlayer1.Y + Program.formMain.ilPlayerAvatarsBig.ImageSize.Height + 2, Program.formMain.ilPlayerAvatarsBig.ImageSize.Width, 6);

            // Считаем максимальное количество здоровья у героев игроков
            maxHealthPlayer1 = CalcHealthPlayer(b.Player1);
            maxHealthPlayer2 = CalcHealthPlayer(b.Player2);

            //
            sizeCell = phb.Size;
            sizeTile = new Size(phb.Size.Width + (FormMain.Config.GridSize * 2) + 1, phb.Size.Height + (FormMain.Config.GridSize * 2) + 1);
            topLeftGrid = new Point(FormMain.Config.GridSize, rectBandHealthPlayer1.Y + rectBandHealthPlayer1.Height + FormMain.Config.GridSize);
            topLeftCells = new Point(topLeftGrid.X + FormMain.Config.GridSize + 1, topLeftGrid.Y + FormMain.Config.GridSize + 1);

            cellHeroes = new PanelHeroInBattle[b.SizeBattlefield.Height, b.SizeBattlefield.Width];
            for (int y = 0; y < b.SizeBattlefield.Height; y++)
                for (int x = 0; x < b.SizeBattlefield.Width; x++)
                {
                    p = new PanelHeroInBattle(Program.formMain.ilGuiHeroes)
                    {
                        //Parent = this,
                        Left = topLeftCells.X + (x * sizeTile.Width),
                        Top = topLeftCells.Y + (y * sizeTile.Height)
                    };

                    cellHeroes[y, x] = p;
                }

            Width = topLeftGrid.X + (b.SizeBattlefield.Width * sizeTile.Width) + WIDTH_LINE + FormMain.Config.GridSize + (Width - ClientSize.Width);
            Height = topLeftGrid.Y + (b.SizeBattlefield.Height * sizeTile.Height + WIDTH_LINE) + FormMain.Config.GridSize +  (Height - ClientSize.Height);

            // Положение аватарки второго игрока
            pointAvatarPlayer2 = new Point(ClientSize.Width - Program.formMain.ilPlayerAvatarsBig.ImageSize.Width - FormMain.Config.GridSize, pointAvatarPlayer1.Y);
            lblPlayer2.Left = pointAvatarPlayer2.X;
            lblPlayer2.Text = battle.Player2.Name;
            rectBandHealthPlayer2 = new Rectangle(pointAvatarPlayer2.X, pointAvatarPlayer2.Y + Program.formMain.ilPlayerAvatarsBig.ImageSize.Height + 2, Program.formMain.ilPlayerAvatarsBig.ImageSize.Width, 6);

            //
            lblStateBattle.Top = pointAvatarPlayer1.Y;
            lblStateBattle.Width = pointAvatarPlayer2.X - pointAvatarPlayer1.X - Program.formMain.ilPlayerAvatarsBig.ImageSize.Width - FormMain.Config.GridSize * 2;
            lblStateBattle.Left = pointAvatarPlayer1.X + Program.formMain.ilPlayerAvatarsBig.ImageSize.Width + FormMain.Config.GridSize;

            lblTimer.Top = lblStateBattle.Top + lblStateBattle.Height;
            lblTimer.Width = lblStateBattle.Width;
            lblTimer.Left = lblStateBattle.Left;

            btnEndBattle.Top = lblTimer.Top + lblTimer.Height;
            btnEndBattle.Width = 136;
            btnEndBattle.Height = 32;
            btnEndBattle.Left = pointAvatarPlayer1.X + Program.formMain.ilPlayerAvatarsBig.ImageSize.Width + (((pointAvatarPlayer2.X - pointAvatarPlayer1.X - Program.formMain.ilPlayerAvatarsBig.ImageSize.Width) - btnEndBattle.Width) / 2);

            btnDecSpeed.Top = GuiUtils.NextTop(btnEndBattle);
            btnDecSpeed.Left = btnEndBattle.Left;
            btnIncSpeed.Top = btnDecSpeed.Top;
            btnIncSpeed.Left = btnEndBattle.Left + btnEndBattle.Width - btnIncSpeed.Width;
            btnPlayPause.Top = btnDecSpeed.Top;
            btnPlayPause.Left = btnDecSpeed.Left + btnDecSpeed.Width;
            btnPlayPause.Width = btnIncSpeed.Left - btnPlayPause.Left;
            Debug.Assert(btnPlayPause.Width > 0);

            //
            lblDamagePlayer1.Top = btnEndBattle.Top;
            lblDamagePlayer1.Left = pointAvatarPlayer1.X + Program.formMain.ilPlayerAvatarsBig.ImageSize.Width + FormMain.Config.GridSize;
            lblDamagePlayer1.Hide();
            lblDamagePlayer2.Top = btnEndBattle.Top;
            lblDamagePlayer2.Left = pointAvatarPlayer2.X - FormMain.Config.GridSize - 80;
            lblDamagePlayer2.Hide();

            // Подготавливаем подложку
            bmpBackground = GuiUtils.MakeBackground(ClientSize);

            //
            ApplyStep();

            phb.Dispose();

            ShowDialog();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            timeStart = DateTime.Now;
            timeInternalFixed = timeStart; 
            stepsCalcedByCurrentSpeed = 0;
            timePassedCurrentSpeed.Start();
            Application.DoEvents();

            for (; ; )
            {
                TimerStep_Tick(this, e);
                Application.DoEvents();

                if (needClose)
                    break;

                if (battle.BattleCalced)
                    break;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            needClose = e.Cancel;
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
                //Debug.Assert(cellHeroes[h.Coord.Y, h.Coord.X].Hero == null);

                //cellHeroes[h.Coord.Y, h.Coord.X].Hero = h;
            }
            
            btnEndBattle.Enabled = !battle.BattleCalced;
            btnDecSpeed.Enabled = btnEndBattle.Enabled;
            btnIncSpeed.Enabled = btnEndBattle.Enabled;
            btnPlayPause.Enabled = btnEndBattle.Enabled;

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

            Refresh();// Увеличивает потребление ЦП
        }
    }
}
