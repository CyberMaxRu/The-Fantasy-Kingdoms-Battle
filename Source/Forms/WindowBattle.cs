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

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowBattle : VCForm
    {
        enum SpeedBattle { Minimal, VerySlow, Slow, Normal, Fast, VeryFast, Maximal };

        private Battle battle;
        private Pen penArrow;
        private Pen penGrid = new Pen(FormMain.Config.BattlefieldGrid);
        private Size sizeTile;
        private Size sizeCell;
        private Point topLeftGrid;
        private Point topLeftCells;
        private const int WIDTH_LINE = 1;
        private bool showGrid;

        private Timer timerStep;
        private bool inStep = false;

        // Время битвы
        private SpeedBattle currentSpeed = SpeedBattle.Normal;
        private bool inPause = false;
        private readonly List<DateTime> Frames = new List<DateTime>();
        private readonly List<DateTime> Steps = new List<DateTime>();
        private DateTime timeStart;// Время начала битвы
        private DateTime timeInternalFixed;// Внутреннее время битвы, с учетом изменения скорости
        private DateTime timeInternalApprox;// Ориентировочное время битвы
        private int stepsCalcedByCurrentSpeed;// Сколько шагов посчитано по текущей скорости
        private Stopwatch timePassedCurrentSpeed = new Stopwatch();// Время, прошедшее с начала задействования текущей скорости
        private Stopwatch timeBetweenFrames = new Stopwatch();// Время между перерисовками кадром, чтобы избежать слишком частого перерисовывания

        private bool needClose = false;

        private readonly SolidBrush brushHealth = new SolidBrush(FormMain.Config.BattlefieldPlayerHealth);
        private readonly SolidBrush brushNoneHealth = new SolidBrush(FormMain.Config.BattlefieldPlayerHealthNone);

        private int maxHealthPlayer1;
        private int maxHealthPlayer2;

        private readonly VCImage128 imgAvatarParticipant1;
        private readonly VCImage128 imgAvatarParticipant2;
        private readonly VCText lblPlayer1;
        private readonly VCText lblPlayer2;
        private Rectangle rectBandHealthPlayer1;
        private Rectangle rectBandHealthPlayer2;
        private readonly VCLabel lblSystemInfo;
        private readonly VCLabel lblStateBattle;
        private readonly VCLabel lblTimer;
        private readonly VCLabel lblDamagePlayer1;
        private readonly VCLabel lblDamagePlayer2;
        private readonly VCButton btnEndBattle;
        private readonly VCButton btnPlayPause;
        private readonly VCIconButton24 btnDecSpeed;
        private readonly VCIconButton24 btnIncSpeed;
        private readonly VCCheckBox chkbShowGrid;
        private readonly VCCheckBox chkbShowPath;

        public WindowBattle(Battle b)
        {
            battle = b;
            windowCaption.Caption = b.Player1.GetName() + " vs " + b.Player2.GetName();

            // Расчет координат для отрисовки
            sizeCell = Program.formMain.bmpBorderForIcon.Size;

            sizeTile = new Size(sizeCell.Width + (FormMain.Config.GridSize * 2) + 1, sizeCell.Height + (FormMain.Config.GridSize * 2) + 1);

            ClientControl.Width = FormMain.Config.GridSize + (b.SizeBattlefield.Width * sizeTile.Width) + WIDTH_LINE + FormMain.Config.GridSize;

            //
            penArrow = new Pen(FormMain.Config.BattlefieldAllyColor)
            {
                Width = 2,
                CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(3.0F, 6.0F, true)
            };

            // Создаем контролы
            lblSystemInfo = new VCLabel(ClientControl, 0, FormMain.Config.GridSize, Program.formMain.FontParagraph, FormMain.Config.BattlefieldSystemInfo, 24, "");
            lblSystemInfo.Width = 560;
            lblSystemInfo.StringFormat.Alignment = StringAlignment.Near;
            lblSystemInfo.StringFormat.LineAlignment = StringAlignment.Center;

            lblPlayer1 = new VCText(ClientControl, FormMain.Config.GridSize, lblSystemInfo.NextTop(), Program.formMain.FontMedCaptionC, FormMain.Config.BattlefieldPlayerName, Program.formMain.BmpListObjects128.Size.Width);
            lblPlayer1.Height = 48;
            lblPlayer1.StringFormat.Alignment = StringAlignment.Center;
            lblPlayer1.StringFormat.LineAlignment = StringAlignment.Far;
            lblPlayer1.Text = battle.Player1.GetName();

            lblPlayer2 = new VCText(ClientControl, ClientControl.Width - Program.formMain.BmpListObjects128.Size.Width - FormMain.Config.GridSize, lblPlayer1.ShiftY, Program.formMain.FontMedCaptionC, FormMain.Config.BattlefieldPlayerName, Program.formMain.BmpListObjects128.Size.Width);
            lblPlayer2.Height = 48;
            lblPlayer2.StringFormat.Alignment = StringAlignment.Center;
            lblPlayer2.StringFormat.LineAlignment = StringAlignment.Far;
            lblPlayer2.Text = battle.Player2.GetName();

            imgAvatarParticipant1 = new VCImage128(ClientControl, FormMain.Config.GridSize, lblPlayer1.NextTop());
            imgAvatarParticipant1.ShiftX = lblPlayer1.ShiftX;
            imgAvatarParticipant1.ImageIndex = b.Player1.GetImageIndexAvatar();
            imgAvatarParticipant2 = new VCImage128(ClientControl, FormMain.Config.GridSize, lblPlayer2.NextTop());
            imgAvatarParticipant2.ShiftX = lblPlayer2.ShiftX;
            imgAvatarParticipant2.ImageIndex = b.Player2.GetImageIndexAvatar();

            rectBandHealthPlayer1 = new Rectangle(imgAvatarParticipant1.ShiftX, imgAvatarParticipant2.ShiftY + Program.formMain.BmpListObjects128.Size.Width + 2, Program.formMain.BmpListObjects128.Size.Height, 6);
            rectBandHealthPlayer2 = new Rectangle(imgAvatarParticipant2.ShiftX, imgAvatarParticipant1.ShiftY + Program.formMain.BmpListObjects128.Size.Width + 2, Program.formMain.BmpListObjects128.Size.Height, 6);

            topLeftGrid = new Point(FormMain.Config.GridSize, rectBandHealthPlayer1.Y + rectBandHealthPlayer1.Height + FormMain.Config.GridSize);
            topLeftCells = new Point(topLeftGrid.X + FormMain.Config.GridSize + 1, topLeftGrid.Y + FormMain.Config.GridSize + 1);
            ClientControl.Height = topLeftGrid.Y + (battle.SizeBattlefield.Height * sizeTile.Height + WIDTH_LINE) + FormMain.Config.GridSize;

            chkbShowGrid = new VCCheckBox(ClientControl, ClientControl.Width - 80 - FormMain.Config.GridSize, lblSystemInfo.ShiftY, "Сетка");
            chkbShowGrid.Width = 80;
            chkbShowGrid.Checked = Program.formMain.Settings.BattlefieldShowGrid;
            showGrid = !chkbShowGrid.Checked;

            chkbShowPath = new VCCheckBox(ClientControl, 0, chkbShowGrid.ShiftY, "Путь");
            chkbShowPath.Width = 80;
            chkbShowPath.ShiftX = chkbShowGrid.ShiftX - chkbShowPath.Width - FormMain.Config.GridSize;
            chkbShowPath.Checked = Program.formMain.Settings.BattlefieldShowPath;

            lblStateBattle = new VCLabel(ClientControl, 0, imgAvatarParticipant1.ShiftX,
                Program.formMain.FontMedCaptionC, FormMain.Config.BattlefieldPlayerName, 32, "Идет бой");
            lblStateBattle.Width = 160;
            lblStateBattle.ShiftX = (ClientControl.Width - lblStateBattle.Width) / 2;
            lblStateBattle.StringFormat.Alignment = StringAlignment.Center;
            lblStateBattle.StringFormat.LineAlignment = StringAlignment.Center;

            lblTimer = new VCLabel(ClientControl, lblStateBattle.ShiftX, lblStateBattle.NextTop(), Program.formMain.FontMedCaptionC, FormMain.Config.BattlefieldPlayerName, 32, "");
            lblTimer.Width = lblStateBattle.Width;
            lblTimer.StringFormat.Alignment = StringAlignment.Center;
            lblTimer.StringFormat.LineAlignment = StringAlignment.Center;

            lblDamagePlayer1 = new VCLabel(ClientControl, 0, 0, Program.formMain.FontParagraph, FormMain.Config.BattlefieldPlayerName, 24, "", Program.formMain.BmpListGui24);
            lblDamagePlayer1.Image.ImageIndex = FormMain.GUI_24_STAR;
            lblDamagePlayer1.Width = 64;
            lblDamagePlayer1.StringFormat.Alignment = StringAlignment.Far;
            lblDamagePlayer1.StringFormat.LineAlignment = StringAlignment.Center;

            lblDamagePlayer2 = new VCLabel(ClientControl, 0, 0, Program.formMain.FontParagraph, FormMain.Config.BattlefieldPlayerName, 24, "", Program.formMain.BmpListGui24);
            lblDamagePlayer2.Image.ImageIndex = FormMain.GUI_24_STAR;
            lblDamagePlayer2.Width = 64;
            lblDamagePlayer2.StringFormat.Alignment = StringAlignment.Far;
            lblDamagePlayer2.StringFormat.LineAlignment = StringAlignment.Center;

            btnEndBattle = new VCButton(ClientControl, 0, lblTimer.NextTop(), "Завершить бой");
            btnEndBattle.Width = 192;
            btnEndBattle.ShiftX = imgAvatarParticipant1.ShiftX + Program.formMain.BmpListObjects128.Size.Width + (((imgAvatarParticipant2.ShiftX - imgAvatarParticipant1.ShiftX - Program.formMain.BmpListObjects128.Size.Width) - btnEndBattle.Width) / 2);
            btnEndBattle.Click += BtnEndBattle_Click;

            btnPlayPause = new VCButton(ClientControl, btnEndBattle.ShiftX, btnEndBattle.NextTop(), "Скорость");
            btnPlayPause.Width = btnEndBattle.Width;
            btnPlayPause.Click += BtnPlayPause_Click;
            //btnPlayPause.Width = btnIncSpeed.ShiftX - btnPlayPause.ShiftX;
            Debug.Assert(btnPlayPause.Width > 0);

            btnDecSpeed = VCIconButton24.CreateButton(ClientControl, 0, 0, FormMain.GUI_24_BUTTON_LEFT, BtnDecSpeed_Click);
            btnDecSpeed.ShiftX = btnPlayPause.ShiftX - btnDecSpeed.Width;
            btnDecSpeed.ShiftY = btnPlayPause.ShiftY + (btnPlayPause.Height - btnDecSpeed.Height) / 2 + 1;

            btnIncSpeed = VCIconButton24.CreateButton(ClientControl, btnPlayPause.ShiftX + btnPlayPause.Width, btnDecSpeed.ShiftY, FormMain.GUI_24_BUTTON_RIGHT, BtnIncSpeed_Click);

            ApplySpeed();

            // Считаем максимальное количество здоровья у героев игроков
            maxHealthPlayer1 = CalcHealthPlayer(b.Player1);
            maxHealthPlayer2 = CalcHealthPlayer(b.Player2);

            //
            lblDamagePlayer1.ShiftX = imgAvatarParticipant1.ShiftX + Program.formMain.BmpListObjects128.Size.Width + FormMain.Config.GridSize;
            lblDamagePlayer1.ShiftY = btnEndBattle.ShiftY;
            lblDamagePlayer1.Visible = false;
            lblDamagePlayer2.ShiftX = imgAvatarParticipant2.ShiftX - FormMain.Config.GridSize - 80;
            lblDamagePlayer2.ShiftY = btnEndBattle.ShiftY;
            lblDamagePlayer2.Visible = false;
        }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            rectBandHealthPlayer1 = new Rectangle(imgAvatarParticipant1.Left, imgAvatarParticipant1.Top + Program.formMain.BmpListObjects128.Size.Width + 2, Program.formMain.BmpListObjects128.Size.Height, 6);
            rectBandHealthPlayer2 = new Rectangle(imgAvatarParticipant2.Left, imgAvatarParticipant2.Top + Program.formMain.BmpListObjects128.Size.Width + 2, Program.formMain.BmpListObjects128.Size.Height, 6);

            topLeftGrid = new Point(FormMain.Config.GridSize, rectBandHealthPlayer1.Y + rectBandHealthPlayer1.Height + FormMain.Config.GridSize);
            topLeftCells = new Point(topLeftGrid.X + FormMain.Config.GridSize + 1, topLeftGrid.Y + FormMain.Config.GridSize + 1);
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
                case SpeedBattle.Minimal:
                    currentSpeed = SpeedBattle.VerySlow;
                    break;
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
                case SpeedBattle.VeryFast:
                    currentSpeed = SpeedBattle.Maximal;
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
                case SpeedBattle.VerySlow:
                    currentSpeed = SpeedBattle.Minimal;
                    break;
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
                case SpeedBattle.Maximal:
                    currentSpeed = SpeedBattle.VeryFast;
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

                timePassedCurrentSpeed.Start();
            }

            ApplySpeed();
        }

        private void ApplySpeed()
        {
            if (inPause)
            {
                btnPlayPause.Caption = "Пауза";
            }
            else
            {
                btnPlayPause.Caption = "Скорость " + ValueSpeed().ToString() + "x";
            }

            btnDecSpeed.ImageIsEnabled = !inPause && !battle.BattleCalced && (currentSpeed != SpeedBattle.Minimal);
            btnIncSpeed.ImageIsEnabled = !inPause && !battle.BattleCalced && (currentSpeed != SpeedBattle.Maximal);
        }

        private double ValueSpeed()
        {
            switch (currentSpeed)
            {
                case SpeedBattle.Minimal:
                    return 0.1;
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
                case SpeedBattle.Maximal:
                    return 10;
                default:
                    throw new Exception("Неизвестная скорость.");
            }
        }

        private void BtnEndBattle_Click(object sender, EventArgs e)
        {
            if (!battle.BattleCalced)
            {
                battle.CalcWholeBattle();

                ApplyStep();
                ShowResultBattle();
                ShowTimerBattle();
            }
            else
            {
                CloseForm(DialogAction.None);
            }
        }

        private void TimerStep_Tick(object sender, EventArgs e)
        {
            if (Program.formMain.ProgramState == ProgramState.ConfirmQuit)
                return;

            if (inStep)
                return;

            if (inPause)
                return;

            inStep = true;
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

            inStep = false;
        }

        private void ShowFrame()
        {
            if (timeBetweenFrames.ElapsedMilliseconds >= FormMain.Config.MaxDurationFrame)
            {
                timeBetweenFrames.Restart();

                DrawFps();
                Frames.Add(DateTime.Now);

                ApplyStep();

                Program.formMain.ShowFrame(true);
            }
            //else
            //    System.Threading.Thread.Sleep((FormMain.Config.MaxDurationFrame - (int)timeBetweenFrames.ElapsedMilliseconds));
        }

        private Point CellToClientCoord(BattlefieldTile t)
        {
            return new Point(topLeftCells.X + (t.X * sizeTile.Width), topLeftCells.Y + (t.Y * sizeTile.Height));
        }

        internal override void Draw(Graphics g)
        {
            // Обновляем аватарки игроков
            imgAvatarParticipant1.ImageIsEnabled = (battle.BattleCalced == false) || (battle.Winner == battle.Player1);
            imgAvatarParticipant2.ImageIsEnabled = (battle.BattleCalced == false) || (battle.Winner == battle.Player2);

            base.Draw(g);

            if (chkbShowGrid == null)
                return;
            showGrid = chkbShowGrid.Checked;

            // Рисуем сетку
            if (showGrid)
            {
                // Вертикальные линии
                for (int x = 0; x <= battle.SizeBattlefield.Width; x++)
                    g.DrawLine(penGrid,ClientControl.Left + topLeftGrid.X + x * sizeTile.Width, topLeftGrid.Y, ClientControl.Left + topLeftGrid.X + x * sizeTile.Width, topLeftGrid.Y + battle.SizeBattlefield.Height * sizeTile.Height);
                // Горизонтальные линии
                for (int y = 0; y <= battle.SizeBattlefield.Height; y++)
                    g.DrawLine(penGrid, ClientControl.Left + topLeftGrid.X, topLeftGrid.Y + y * sizeTile.Height, ClientControl.Left + topLeftGrid.X + battle.SizeBattlefield.Width * sizeTile.Width, topLeftGrid.Y + y * sizeTile.Height);
            }

            // Рисуем полоски жизней героев игроков
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            GuiUtils.DrawBand(g, rectBandHealthPlayer1, brushHealth, brushNoneHealth, CalcHealthPlayer(battle.Player1), maxHealthPlayer1);
            GuiUtils.DrawBand(g, rectBandHealthPlayer2, brushHealth, brushNoneHealth, CalcHealthPlayer(battle.Player2), maxHealthPlayer2);

            // Рисуем героев
            foreach (HeroInBattle hero in battle.ActiveHeroes)
            {
                Point coordIconHero = CellToClientCoord(hero.CurrentTile);
                Point shift = new Point(0, 0);

                if (hero.TileForMove != null)
                {
                    BattlefieldTile tileforMove = hero.TileForMove;
                    Point coordTileForMove = CellToClientCoord(tileforMove);
                    Debug.Assert(hero.CurrentTile.IsNeighbourTile(tileforMove));
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
                g.DrawImageUnscaled(hero.BmpIcon, coordIconHero.X + shift.X, coordIconHero.Y + shift.Y);

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
                                penArrow.Color = hero.PlayerHero.BattleParticipant == battle.Player1 ? FormMain.Config.BattlefieldAllyColor : FormMain.Config.BattlefieldEnemyColor;
                                pTarget = new Point(topLeftGrid.X + (t.Coord.X * sizeTile.Width) + (sizeTile.Width / 2), topLeftGrid.Y + (t.Coord.Y * sizeTile.Height) + (sizeTile.Height / 2) + WIDTH_LINE);
                                g.DrawLine(penArrow, pSource, pTarget);

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

                m.Draw(g, p1, p2);
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
                lblDamagePlayer1.Visible = true;
                //lblDamagePlayer1.Text = battle.Player1.LastBattleDamageToCastle.ToString();
            }
            else if (battle.Winner == battle.Player2)
            {
                lblDamagePlayer2.Visible = true;
                //lblDamagePlayer2.Text = battle.Player2.LastBattleDamageToCastle.ToString();
            }

            // Показываем состояние
            if ((battle.Winner != null) && (battle.Winner.GetTypePlayer() == TypePlayer.Human))
            {
                lblStateBattle.Text = "Победа!";
                lblStateBattle.Color = FormMain.Config.BattlefieldTextWin;
            }
            else if (battle.Winner == null)
            {
                lblStateBattle.Text = "Ничья";
                lblStateBattle.Color = FormMain.Config.BattlefieldTextDraw;
            }
            else
            {
                lblStateBattle.Text = "Поражение";
                lblStateBattle.Color = FormMain.Config.BattlefieldTextLose;
            }

            btnPlayPause.Enabled = false;
            btnDecSpeed.ImageIsEnabled = false;
            btnIncSpeed.ImageIsEnabled = false;
        }

        protected override void BeforeClose(DialogAction da)
        {
            base.BeforeClose(da);

            Program.formMain.Settings.BattlefieldShowGrid = chkbShowGrid.Checked;
            Program.formMain.Settings.BattlefieldShowPath = chkbShowPath.Checked;

            if (battle.BattleCalced == false)
                battle.CalcWholeBattle();
        }

        private void DrawFps()
        {
            // Считаем Frames per second
            // Для этого удаляем все кадры, которые были более секунды назад, добавляем текущий и получаем итоговое количество
            DateTime currentDateTime = DateTime.Now;
            Utils.TrimActions(Frames);
            Utils.TrimActions(Steps);

            TimeSpan realTime = currentDateTime - timeStart;

            lblSystemInfo.Text = Frames.Count.ToString() + " fps; steps: " + Steps.Count.ToString()
                  + "; passed: " + realTime.ToString("mm':'ss");
        }

        internal void ShowBattle()
        {
            //
            ApplyStep();

            //
            timeStart = DateTime.Now;
            timeInternalFixed = timeStart;
            stepsCalcedByCurrentSpeed = 0;
            timePassedCurrentSpeed.Start();
            timeBetweenFrames.Start();

            timerStep = new Timer();
            timerStep.Interval = 10;
            timerStep.Tick += TimerStep_Tick;
            timerStep.Start();

            ShowDialog();
        }

        private int CalcHealthPlayer(BattleParticipant p)
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
            //btnEndBattle.Enabled = !battle.BattleCalced;
            btnEndBattle.Caption = battle.BattleCalced ? "Закрыть" : "Завершить бой";
            btnPlayPause.Enabled = !battle.BattleCalced;
        }
    }
}
