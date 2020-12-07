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

        private Battle battle;
        private Pen penArrow;
        private Bitmap bmpBackground;
        private Bitmap bmpLayBackground;
        private Bitmap bmpFrame;
        private Graphics gFrame;
        private Pen penGrid = new Pen(FormMain.Config.BattlefieldGrid);
        private Size sizeTile;
        private Size sizeCell;
        private Point topLeftGrid;
        private Point topLeftCells;
        private const int WIDTH_LINE = 1;
        private CheckBox chkbShowGrid;
        private CheckBox chkbShowPath;
        private bool showGrid;
        private int widthBorder;
        private int lengthBandBorder;
        private Rectangle rectBorderBattlefield;
        private Rectangle rectCornerBorder;
        private Rectangle rectHorBandBorder;
        private Rectangle rectVertBandBorder;

        // Время битвы
        private SpeedBattle currentSpeed = SpeedBattle.Normal;
        private bool inPause = false;
        private readonly List<DateTime> Frames = new List<DateTime>();
        private readonly List<DateTime> Steps = new List<DateTime>();
        private readonly List<DateTime> BackPaints = new List<DateTime>();
        private DateTime timeStart;// Время начала битвы
        private DateTime timeInternalFixed;// Внутреннее время битвы, с учетом изменения скорости
        private DateTime timeInternalApprox;// Ориентировочное время битвы
        private int stepsCalcedByCurrentSpeed;// Сколько шагов посчитано по текущей скорости
        private Stopwatch timePassedCurrentSpeed = new Stopwatch();// Время, прошедшее с начала задействования текущей скорости
        private Stopwatch timeBetweenFrames = new Stopwatch();// Время между перерисовками кадром, чтобы избежать слишком частого перерисовывания

        private bool needClose = false;

        private readonly Label lblSystemInfo;
        private readonly Label lblPlayer1;
        private readonly Label lblPlayer2;
        private Point pointAvatarPlayer1;
        private Point pointAvatarPlayer2;
        private Rectangle rectBandHealthPlayer1;
        private Rectangle rectBandHealthPlayer2;
        private readonly SolidBrush brushHealth = new SolidBrush(FormMain.Config.BattlefieldPlayerHealth);
        private readonly SolidBrush brushNoneHealth = new SolidBrush(FormMain.Config.BattlefieldPlayerHealthNone);
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

            FormClosing += FormBattle_FormClosing;

            penArrow = new Pen(FormMain.Config.BattlefieldAllyColor)
            {
                Width = 2,
                CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(3.0F, 6.0F, true)
            };

            // Создаем контролы
            lblSystemInfo = new Label()
            {
                Parent = this,
                Top = FormMain.Config.GridSize,
                Width = 560,
                ForeColor = FormMain.Config.BattlefieldSystemInfo,
                BackColor = Color.Transparent,
                Height = 24,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = FormMain.Config.FontBattlefieldPlayer
            };

            chkbShowGrid = new CheckBox()
            {
                Parent = this,
                AutoSize = true,
                Top = lblSystemInfo.Top,
                Text = "Показать сетку",
                ForeColor = FormMain.Config.BattlefieldSystemInfo,
                BackColor = Color.Transparent
            };

            chkbShowPath = new CheckBox()
            {
                Parent = this,
                AutoSize = true,
                Top = lblSystemInfo.Top,
                Text = "Показать путь",
                ForeColor = FormMain.Config.BattlefieldSystemInfo,
                BackColor = Color.Transparent
            };

            lblPlayer1 = new Label()
            {
                Parent = this,
                Top = GuiUtils.NextTop(lblSystemInfo),
                ForeColor = FormMain.Config.BattlefieldPlayerName,
                BackColor = Color.Transparent,
                AutoSize = false,
                Width = Program.formMain.ilPlayerAvatarsBig.ImageSize.Width,
                Height = 24,
                TextAlign = ContentAlignment.BottomCenter,
                Font = FormMain.Config.FontBattlefieldPlayer
            };

            lblPlayer2 = new Label()
            {
                Parent = this,
                Top = lblPlayer1.Top,
                ForeColor = FormMain.Config.BattlefieldPlayerName,
                BackColor = Color.Transparent,
                AutoSize = false,
                Width = Program.formMain.ilPlayerAvatarsBig.ImageSize.Width,
                Height = 24,
                TextAlign = ContentAlignment.BottomCenter,
                Font = FormMain.Config.FontBattlefieldPlayer
            };

            lblStateBattle = new Label()
            {
                Parent = this,
                ForeColor = FormMain.Config.BattlefieldPlayerName,
                BackColor = Color.Transparent,
                AutoSize = false,
                Height = 32,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = FormMain.Config.FontBattlefieldTimer
            };

            lblTimer = new Label()
            {
                Parent = this,
                ForeColor = FormMain.Config.BattlefieldPlayerName,
                BackColor = Color.Transparent,
                AutoSize = false,
                Height = 32,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = FormMain.Config.FontBattlefieldTimer
            };

            lblDamagePlayer1 = new Label()
            {
                Parent = this,
                ForeColor = FormMain.Config.BattlefieldPlayerName,
                BackColor = Color.Transparent,
                AutoSize = false,
                ImageList = Program.formMain.ilGui24,
                ImageIndex = FormMain.GUI_24_STAR,
                Height = 24,
                Width = 64,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = FormMain.Config.FontBattlefieldDamage
            };

            lblDamagePlayer2 = new Label()
            {
                Parent = this,
                ForeColor = FormMain.Config.BattlefieldPlayerName,
                BackColor = Color.Transparent,
                AutoSize = false,
                ImageList = Program.formMain.ilGui24,
                ImageIndex = FormMain.GUI_24_STAR,
                Height = 24,
                Width = 64,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = FormMain.Config.FontBattlefieldDamage
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
            battle.CalcWholeBattle(null);

            ApplyStep();
            ShowResultBattle();
            ShowTimerBattle();
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
                        {
                            ShowResultBattle();

                            break;
                        }
                    }

                    ShowFrame();
                }
            }
        }

        private void ShowFrame()
        {
            if (timeBetweenFrames.ElapsedMilliseconds >= FormMain.Config.MaxDurationFrame)
            {
                timeBetweenFrames.Restart();

                DrawFps();
                Frames.Add(DateTime.Now);
                
                ApplyStep();
                DrawFrame();
                Invalidate();
                Refresh();
            }
            //else
            //    System.Threading.Thread.Sleep((FormMain.Config.MaxDurationFrame - (int)timeBetweenFrames.ElapsedMilliseconds));
        }

        private void DrawBackground()
        {
            Graphics g = Graphics.FromImage(bmpLayBackground);

            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            g.DrawImageUnscaled(bmpBackground, 0, 0);

            // Рисуем границу поля боя
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            // Левый верхний угол
            g.DrawImage(Program.formMain.bmpBorderBattlefield,
                    new Rectangle(rectBorderBattlefield.X, rectBorderBattlefield.Y, widthBorder, widthBorder),
                    new Rectangle(0, 0, widthBorder, widthBorder), GraphicsUnit.Pixel);

            // Правый верхний угол
            g.DrawImage(Program.formMain.bmpBorderBattlefield,
                    new Rectangle(rectBorderBattlefield.X + rectBorderBattlefield.Width - widthBorder, rectBorderBattlefield.Y, widthBorder, widthBorder),
                    new Rectangle(widthBorder + lengthBandBorder, 0, widthBorder, widthBorder), GraphicsUnit.Pixel);

            // Левый нижний угол
            g.DrawImage(Program.formMain.bmpBorderBattlefield,
                    new Rectangle(rectBorderBattlefield.X, rectBorderBattlefield.Y + rectBorderBattlefield.Height - widthBorder, widthBorder, widthBorder),
                    new Rectangle(0, widthBorder + lengthBandBorder, widthBorder, widthBorder), GraphicsUnit.Pixel);

            // Правый нижний угол
            g.DrawImage(Program.formMain.bmpBorderBattlefield,
                    new Rectangle(rectBorderBattlefield.X + rectBorderBattlefield.Width - widthBorder, rectBorderBattlefield.Y + rectBorderBattlefield.Height - widthBorder, widthBorder, widthBorder),
                    new Rectangle(widthBorder + lengthBandBorder, widthBorder + lengthBandBorder, widthBorder, widthBorder), GraphicsUnit.Pixel);

            // Горизонтальные бордюры
            int repeats = (rectBorderBattlefield.Width - (widthBorder * 2)) / lengthBandBorder;
            int restBorder = (rectBorderBattlefield.Width - (widthBorder * 2)) % lengthBandBorder;
            Rectangle rectTopBorder = new Rectangle(widthBorder, 0, lengthBandBorder, widthBorder);
            Rectangle rectBottomBorder = new Rectangle(widthBorder, widthBorder + lengthBandBorder, lengthBandBorder, widthBorder);

            for (int i = 0; i < repeats; i++)
            {
                // Верхний бордюр
                g.DrawImage(Program.formMain.bmpBorderBattlefield,
                        new Rectangle(rectBorderBattlefield.X + widthBorder + (i * lengthBandBorder), rectBorderBattlefield.Y, lengthBandBorder, widthBorder),
                        rectTopBorder, GraphicsUnit.Pixel);

                // Нижний бордюр
                g.DrawImage(Program.formMain.bmpBorderBattlefield,
                        new Rectangle(rectBorderBattlefield.X + widthBorder + (i * lengthBandBorder), rectBorderBattlefield.Y + rectBorderBattlefield.Height - widthBorder, lengthBandBorder, widthBorder),
                        rectBottomBorder, GraphicsUnit.Pixel);
            }

            // Верхний бордюр
            g.DrawImage(Program.formMain.bmpBorderBattlefield,
                    new Rectangle(rectBorderBattlefield.X + widthBorder + (repeats * lengthBandBorder), rectBorderBattlefield.Y, restBorder, widthBorder),
                    rectTopBorder, GraphicsUnit.Pixel);

            // Нижний бордюр
            g.DrawImage(Program.formMain.bmpBorderBattlefield,
                    new Rectangle(rectBorderBattlefield.X + widthBorder + (repeats * lengthBandBorder), rectBorderBattlefield.Y + rectBorderBattlefield.Height - widthBorder, restBorder, widthBorder),
                    rectBottomBorder, GraphicsUnit.Pixel);

            // Вертикальные бордюры
            repeats = (rectBorderBattlefield.Height - (widthBorder * 2)) / lengthBandBorder;
            restBorder = (rectBorderBattlefield.Height - (widthBorder * 2)) % lengthBandBorder;
            Rectangle rectLeftBorder = new Rectangle(0, widthBorder, widthBorder, lengthBandBorder);
            Rectangle rectRightBorder = new Rectangle(widthBorder + lengthBandBorder, widthBorder, widthBorder, lengthBandBorder);

            for (int i = 0; i < repeats; i++)
            {
                // Левый бордюр
                g.DrawImage(Program.formMain.bmpBorderBattlefield,
                        new Rectangle(rectBorderBattlefield.X, rectBorderBattlefield.Y + widthBorder + (i * lengthBandBorder), widthBorder, lengthBandBorder),
                        rectLeftBorder, GraphicsUnit.Pixel);

                // Правый бордюр
                g.DrawImage(Program.formMain.bmpBorderBattlefield,
                        new Rectangle(rectBorderBattlefield.X + rectBorderBattlefield.Width - widthBorder, rectBorderBattlefield.Y + widthBorder + (i * lengthBandBorder), widthBorder, lengthBandBorder),
                        rectRightBorder, GraphicsUnit.Pixel);
            }

            // Левый бордюр
            g.DrawImage(Program.formMain.bmpBorderBattlefield,
                    new Rectangle(rectBorderBattlefield.X, rectBorderBattlefield.Y + widthBorder + (repeats * lengthBandBorder), widthBorder, restBorder),
                    rectLeftBorder, GraphicsUnit.Pixel);

            // Правый бордюр
            g.DrawImage(Program.formMain.bmpBorderBattlefield,
                    new Rectangle(rectBorderBattlefield.X + rectBorderBattlefield.Width - widthBorder, rectBorderBattlefield.Y + widthBorder + (repeats * lengthBandBorder), widthBorder, restBorder),
                    rectRightBorder, GraphicsUnit.Pixel);

            // Рисуем сетку
            if (showGrid)
            {
                // Вертикальные линии
                for (int x = 0; x <= battle.SizeBattlefield.Width; x++)
                    g.DrawLine(penGrid, topLeftGrid.X + x * sizeTile.Width, topLeftGrid.Y, topLeftGrid.X + x * sizeTile.Width, topLeftGrid.Y + battle.SizeBattlefield.Height * sizeTile.Height);
                // Горизонтальные линии
                for (int y = 0; y <= battle.SizeBattlefield.Height; y++)
                    g.DrawLine(penGrid, topLeftGrid.X, topLeftGrid.Y + y * sizeTile.Height, topLeftGrid.X + battle.SizeBattlefield.Width * sizeTile.Width, topLeftGrid.Y + y * sizeTile.Height);
            }

            // Рисуем аватарки игроков
            g.DrawImageUnscaled(Program.formMain.ilPlayerAvatarsBig.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatarsBig, battle.Player1.ImageIndexAvatar, (battle.BattleCalced == false) || (battle.Winner == battle.Player1))], pointAvatarPlayer1);
            g.DrawImageUnscaled(Program.formMain.ilPlayerAvatarsBig.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatarsBig, battle.Player2.ImageIndexAvatar, (battle.BattleCalced == false) || (battle.Winner == battle.Player2))], pointAvatarPlayer2);

            g.Dispose();
        }

        private Point CellToClientCoord(BattlefieldTile t)
        {
            return new Point(topLeftCells.X + (t.X * sizeTile.Width), topLeftCells.Y + (t.Y * sizeTile.Height));
        }

        private void DrawFrame()
        {
            if (showGrid != chkbShowGrid.Checked)
            {
                showGrid = chkbShowGrid.Checked;
                DrawBackground();
            }

            // Рисуем подложку
            gFrame.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            gFrame.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            gFrame.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            gFrame.DrawImageUnscaled(bmpLayBackground, 0, 0);

            // Рисуем полоски жизней героев игроков
            gFrame.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            GuiUtils.DrawBand(gFrame, rectBandHealthPlayer1, brushHealth, brushNoneHealth, CalcHealthPlayer(battle.Player1), maxHealthPlayer1);
            GuiUtils.DrawBand(gFrame, rectBandHealthPlayer2, brushHealth, brushNoneHealth, CalcHealthPlayer(battle.Player2), maxHealthPlayer2);

            // Рисуем героев
            foreach (HeroInBattle hero in battle.ActiveHeroes)
            {
                Point coordIconHero = CellToClientCoord(hero.CurrentTile);
                Point shift = new Point(0, 0);

                if (hero.TileForMove != null)
                {
                    BattlefieldTile tileforMove = hero.TileForMove;
                    Point coordTileForMove = CellToClientCoord(tileforMove);
                    Debug.Assert(Utils.PointsIsNeighbor(hero.Coord, new Point(tileforMove.X, tileforMove.Y)) == true);
                    double percent = hero.PercentExecuteAction();

                    shift.X = (int)((coordTileForMove.X - coordIconHero.X) * percent);
                    shift.Y = (int)((coordTileForMove.Y - coordIconHero.Y) * percent);
                }

                if (((hero.Target != null) || (hero.LastTarget != default)) && (hero.State == StateHeroInBattle.MeleeAttack) && (hero.DestinationForMove == null))
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

                Debug.Assert(coordIconHero.X + shift.X < Width);
                Debug.Assert(coordIconHero.Y + shift.Y >= topLeftCells.Y);
                Debug.Assert(coordIconHero.Y + shift.Y < Height);

                //cellHeroes[y, x].DrawToBitmap(bmpUnit, new Rectangle(0, 0, bmpUnit.Width, bmpUnit.Height));
                gFrame.DrawImageUnscaled(hero.BmpIcon, coordIconHero.X + shift.X, coordIconHero.Y + shift.Y);

                // Рисуем стрелки атаки
                if ((hero.Target != null) || (hero.LastTarget != default) || (hero.DestinationForMove != null))
                {
                    if ((hero.State == StateHeroInBattle.RangeAttack) || (hero.State != StateHeroInBattle.Cast))
                        if (hero.DestinationForMove == null)
                            if (hero.Target is null)
                                continue;

                    Point coordTarget;
                    if (hero.DestinationForMove == null)
                        coordTarget = hero.Target != null ? hero.Target.Coord : hero.LastTarget;
                    else
                        coordTarget = new Point(hero.DestinationForMove.X, hero.DestinationForMove.Y);

                    Point p2 = CellToClientCoord(battle.Battlefield.Tiles[coordTarget.Y, coordTarget.X]);


                    // Делаем расчет точки назначения в зависимости от процент выполнения удара
                    Point pSource = new Point(coordIconHero.X + sizeCell.Width / 2, coordIconHero.Y + sizeCell.Height / 2);
                    Point pTarget = new Point(p2.X + sizeCell.Width / 2, p2.Y + sizeCell.Height / 2);

                    if (hero.DestinationForMove == null)
                    {
                        double percent = hero.PercentExecuteAction();
                        if (hero.State == StateHeroInBattle.MeleeAttack)
                            if (hero.InRollbackAction() == true)
                                percent = 1 - percent;

                        pTarget.X = (int)(pSource.X + ((pTarget.X - pSource.X) * percent));
                        pTarget.Y = (int)(pSource.Y + ((pTarget.Y - pSource.Y) * percent));
                    }
                    else
                    {
                        if (chkbShowPath.Checked)
                        {
                            pSource = new Point(topLeftGrid.X + (hero.Coord.X * sizeTile.Width) + (sizeTile.Width / 2), topLeftGrid.Y + (hero.Coord.Y * sizeTile.Height) + (sizeTile.Height / 2));
                            pSource.X += shift.X;
                            pSource.Y += shift.Y;

                            // Рисуем путь юнита к цели
                            foreach (BattlefieldTile t in hero.PathToDestination)
                            {
                                penArrow.Color = hero.PlayerHero.Player == battle.Player1 ? FormMain.Config.BattlefieldAllyColor : FormMain.Config.BattlefieldEnemyColor;
                                pTarget = new Point(topLeftGrid.X + (t.Coord.X * sizeTile.Width) + (sizeTile.Width / 2), topLeftGrid.Y + (t.Coord.Y * sizeTile.Height) + (sizeTile.Height / 2) + WIDTH_LINE);
                                gFrame.DrawLine(penArrow, pSource, pTarget);

                                pSource = pTarget;
                            }
                        }
                    }
                }

            }

            // Рисуем снаряды
            foreach (Missile m in battle.Missiles)
            {
                Point ph1 = CellToClientCoord(m.SourceTile);
                Point p1 = new Point(ph1.X + sizeCell.Width / 2, ph1.Y + sizeCell.Height / 2);
                Point ph2 = CellToClientCoord(m.DestTile);
                Point p2 = new Point(ph2.X + sizeCell.Width / 2, ph2.Y + sizeCell.Height / 2);

                m.Draw(gFrame, p1, p2);
            }

            ShowTimerBattle();

            //e.Graphics.DrawImage(bmpLayBackground, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            //e.Graphics.DrawImageUnscaled(bmpLay0, 0, 0);
        }

        private void ShowTimerBattle()
        {
            int pastSeconds = battle.Step / FormMain.Config.StepsInSecond;
            TimeSpan ts = new TimeSpan(0, 0, pastSeconds);
            lblTimer.Text = ts.ToString("mm':'ss");
        }

        private void ShowResultBattle()
        {
            Debug.Assert(battle.BattleCalced);

            // Показываем урон по Замку
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

            // Показываем состояние
            if ((battle.Winner != null) && (battle.Winner.TypePlayer == TypePlayer.Human))
            {
                lblStateBattle.Text = "Победа!";
                lblStateBattle.ForeColor = FormMain.Config.BattlefieldTextWin;
            }
            else if (battle.Winner == null)
            {
                lblStateBattle.Text = "Ничья";
                lblStateBattle.ForeColor = FormMain.Config.BattlefieldTextDraw;
            }
            else
            {
                lblStateBattle.Text = "Поражение";
                lblStateBattle.ForeColor = FormMain.Config.BattlefieldTextLose;
            }
        }

        private void FormBattle_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (battle.BattleCalced == false)
                battle.CalcWholeBattle(null); 
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            BackPaints.Add(DateTime.Now);

            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;            
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            e.Graphics.DrawImage(bmpFrame, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            //e.Graphics.DrawImage(bmpLayBackground, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            //e.Graphics.DrawImageUnscaled(bmpLay0, 0, 0);
        }

        private void DrawFps()
        {
            // Считаем Frames per second
            // Для этого удаляем все кадры, которые были более секунды назад, добавляем текущий и получаем итоговое количество
            DateTime currentDateTime = DateTime.Now;
            Utils.TrimActions(Frames);
            Utils.TrimActions(Steps);
            Utils.TrimActions(BackPaints);

            TimeSpan realTime = currentDateTime - timeStart;
            
            lblSystemInfo.Text = Frames.Count.ToString() + " fps; steps: " + Steps.Count.ToString()
                  + "; backpaints: " + BackPaints.Count.ToString() + "; passed: " + realTime.ToString("mm':'ss"); 
        }

        internal void ShowBattle(Battle b)
        {
            Text = "Бой. " + b.Player1.Name + " vs " + b.Player2.Name;

            battle = b;

            // Расчет координат для отрисовки
            sizeCell = Program.formMain.bmpBorderForIcon.Size;

            Width = FormMain.Config.GridSize + (sizeCell.Width + FormMain.Config.GridSize) * FormMain.Config.HeroRows * 2 + FormMain.Config.GridSize + (Width - ClientSize.Width);

            lblPlayer1.Left = FormMain.Config.GridSize;
            lblPlayer1.Text = battle.Player1.Name;
            pointAvatarPlayer1 = new Point(lblPlayer1.Left, lblPlayer1.Top + lblPlayer1.Height);
            rectBandHealthPlayer1 = new Rectangle(pointAvatarPlayer1.X, pointAvatarPlayer1.Y + Program.formMain.ilPlayerAvatarsBig.ImageSize.Height + 2, Program.formMain.ilPlayerAvatarsBig.ImageSize.Width, 6);

            // Считаем максимальное количество здоровья у героев игроков
            maxHealthPlayer1 = CalcHealthPlayer(b.Player1);
            maxHealthPlayer2 = CalcHealthPlayer(b.Player2);

            //
            sizeTile = new Size(sizeCell.Width + (FormMain.Config.GridSize * 2) + 1, sizeCell.Height + (FormMain.Config.GridSize * 2) + 1);
            topLeftGrid = new Point(FormMain.Config.GridSize + FormMain.Config.WidthBorderBattlefield, rectBandHealthPlayer1.Y + rectBandHealthPlayer1.Height + FormMain.Config.GridSize + FormMain.Config.WidthBorderBattlefield);
            topLeftCells = new Point(topLeftGrid.X + FormMain.Config.GridSize + 1, topLeftGrid.Y + FormMain.Config.GridSize + 1);

            Width = topLeftGrid.X + (b.SizeBattlefield.Width * sizeTile.Width) + WIDTH_LINE + FormMain.Config.GridSize
                + FormMain.Config.WidthBorderBattlefield + (Width - ClientSize.Width);
            Height = topLeftGrid.Y + (b.SizeBattlefield.Height * sizeTile.Height + WIDTH_LINE) + FormMain.Config.GridSize
                + FormMain.Config.WidthBorderBattlefield + (Height - ClientSize.Height);

            // Положение аватарки второго игрока
            pointAvatarPlayer2 = new Point(ClientSize.Width - Program.formMain.ilPlayerAvatarsBig.ImageSize.Width - FormMain.Config.GridSize, pointAvatarPlayer1.Y);
            lblPlayer2.Left = pointAvatarPlayer2.X;
            lblPlayer2.Text = battle.Player2.Name;
            rectBandHealthPlayer2 = new Rectangle(pointAvatarPlayer2.X, pointAvatarPlayer2.Y + Program.formMain.ilPlayerAvatarsBig.ImageSize.Height + 2, Program.formMain.ilPlayerAvatarsBig.ImageSize.Width, 6);

            //
            lblStateBattle.Top = pointAvatarPlayer1.Y;
            lblStateBattle.Width = pointAvatarPlayer2.X - pointAvatarPlayer1.X - Program.formMain.ilPlayerAvatarsBig.ImageSize.Width - FormMain.Config.GridSize * 2;
            lblStateBattle.Left = pointAvatarPlayer1.X + Program.formMain.ilPlayerAvatarsBig.ImageSize.Width + FormMain.Config.GridSize;
            lblStateBattle.Text = "Идет бой";

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

            // Подготавливаем фоновый рисунок
            bmpLayBackground = new Bitmap(bmpBackground);

            //
            chkbShowGrid.Left = ClientSize.Width - chkbShowGrid.Width - FormMain.Config.GridSize;
            chkbShowGrid.Checked = FormMain.ShowGrid;
            showGrid = !chkbShowGrid.Checked;
            chkbShowPath.Left = chkbShowGrid.Left - chkbShowPath.Width - FormMain.Config.GridSize;
            chkbShowPath.Checked = FormMain.ShowPath;

            //
            widthBorder = FormMain.Config.WidthBorderBattlefield;
            lengthBandBorder = Program.formMain.LengthSideBorderBattlefield;

            rectBorderBattlefield = new Rectangle(topLeftGrid.X - widthBorder, topLeftGrid.Y - widthBorder,
                ClientSize.Width - (FormMain.Config.GridSize * 2), ClientSize.Height - topLeftGrid.Y + widthBorder - FormMain.Config.GridSize);

            rectCornerBorder = new Rectangle(0, 0, widthBorder, widthBorder);
            rectHorBandBorder = new Rectangle(0, 0, lengthBandBorder, widthBorder);
            rectVertBandBorder = new Rectangle(0, 0, widthBorder, lengthBandBorder);

            //
            bmpFrame = new Bitmap(bmpLayBackground);
            gFrame = Graphics.FromImage(bmpFrame);

            //
            ApplyStep();

            ShowDialog();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            timeStart = DateTime.Now;
            timeInternalFixed = timeStart; 
            stepsCalcedByCurrentSpeed = 0;
            timePassedCurrentSpeed.Start();
            timeBetweenFrames.Start();
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
            btnEndBattle.Enabled = !battle.BattleCalced;
            btnDecSpeed.Enabled = btnEndBattle.Enabled;
            btnIncSpeed.Enabled = btnEndBattle.Enabled;
            btnPlayPause.Enabled = btnEndBattle.Enabled;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            FormMain.ShowGrid = chkbShowGrid.Checked;
            FormMain.ShowPath = chkbShowPath.Checked;
        }
    }
}
