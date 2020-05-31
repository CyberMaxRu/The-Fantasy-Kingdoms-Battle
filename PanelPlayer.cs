using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс панели игрока (сокращенная информация)
    internal sealed class PanelPlayer : Control
    {
        private readonly Player player;
        private Label lblDamageToCastle;
        private Label lblStrike;
        private Label lblQuantityHeroes;
        private Label lblLevelCastle;
        private Rectangle rectBorder;
        private Point pointIconAvatar;// Координаты для отрисовки аватара игрока
        private Point pointIconResultBattle;// Координаты для иконки результата боя
        private Point pointIconStrike;// Координаты для иконки страйка
        private Point pointIconHeroes;// Координаты для иконки героев
        private readonly Pen penBorder = new Pen(Color.Black);
        private readonly SolidBrush brushCurDurability = new SolidBrush(Color.Green);
        private readonly SolidBrush brushMaxDurability = new SolidBrush(Color.LightGreen);
        private readonly SolidBrush brushBackground = new SolidBrush(Color.White);

        public PanelPlayer(Player p, Control parent) : base()
        {
            player = p;
            player.Panel = this;

            Parent = parent;
            Left = Config.GRID_SIZE;
            DoubleBuffered = true;

            pointIconAvatar = new Point(Config.GRID_SIZE, Config.GRID_SIZE);

            lblLevelCastle = new Label()
            {
                Parent = this,
                Location = pointIconAvatar,
                ForeColor = Color.Yellow,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.TopLeft,
                MaximumSize = new Size(Config.GRID_SIZE * 2, Config.GRID_SIZE * 2),
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            int leftForIcons = pointIconAvatar.X + Program.formMain.ilGuiHeroes.ImageSize.Width + Config.GRID_SIZE;
            int leftForText = leftForIcons + Program.formMain.ilGui24.ImageSize.Width + Config.GRID_SIZE_HALF;

            pointIconResultBattle = new Point(leftForIcons, pointIconAvatar.Y);
            pointIconStrike = new Point(leftForIcons, pointIconResultBattle.Y + Program.formMain.ilResultBattle.ImageSize.Height +  Config.GRID_SIZE_HALF);
            pointIconHeroes = new Point(leftForIcons, pointIconStrike.Y + Program.formMain.ilGui24.ImageSize.Height + Config.GRID_SIZE_HALF);

            lblDamageToCastle = new Label()
            {
                Parent = this,
                Left = leftForText,
                Top = pointIconResultBattle.Y,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                MaximumSize = new Size(Program.formMain.ilResultBattle.ImageSize.Width + Config.GRID_SIZE, Program.formMain.ilResultBattle.ImageSize.Height),
                Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold)
            };

            lblStrike = new Label()
            {
                Parent = this,
                Left = leftForText,
                Top = pointIconStrike.Y,
                ForeColor = Color.Black,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                MaximumSize = Program.formMain.ilGui24.ImageSize,
                Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold)
            };

            lblQuantityHeroes = new Label()
            {
                Parent = this,
                Left = leftForText,
                Top = pointIconHeroes.Y,
                ForeColor = Color.Black,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                MaximumSize = Program.formMain.ilGui24.ImageSize,
                Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold)
            };

            Width = leftForText + lblDamageToCastle.MaximumSize.Width + Config.GRID_SIZE_HALF;
            Height = Math.Max(Config.GRID_SIZE + Program.formMain.ilPlayerAvatars.ImageSize.Height + Config.GRID_SIZE + Config.GRID_SIZE,
                GuiUtils.NextTop(lblQuantityHeroes));

            rectBorder = new Rectangle(0, 0, Width - 1, Height - 1);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            Program.formMain.formHint.ShowHint(this,
                player.Name,
                "Место №" + player.PositionInLobby.ToString(),
                "Уровень Замка: " + player.LevelCastle.ToString() + Environment.NewLine
                    + "Прочность Замка " + player.DurabilityCastle.ToString() + "/" + player.Lobby.TypeLobby.DurabilityCastle.ToString() + Environment.NewLine
                    + "Героев: " + player.QuantityHeroes.ToString() + Environment.NewLine
                    + Environment.NewLine
                    + "Побед: " + player.Wins.ToString() + Environment.NewLine
                    + "Ничьих: " + player.Draws.ToString() + Environment.NewLine
                    + "Поражений: " + player.Loses.ToString() + Environment.NewLine
                    ,
                null,
                0,
                false, 0,
                0, false, null);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Program.formMain.formHint.HideHint();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            
            // Фон панели
            if (player == player.Lobby.CurrentPlayer)
                brushBackground.Color = Color.LightBlue;
            else if (player == player.Lobby.CurrentPlayer.Opponent)
                brushBackground.Color = Color.LightCoral;
            else
                brushBackground.Color = Color.FromKnownColor(KnownColor.Control);

            e.Graphics.FillRectangle(brushBackground, rectBorder);

            // Рамка вокруг панели
            e.Graphics.DrawRectangle(penBorder, rectBorder);

            // Иконка героя
            e.Graphics.DrawImageUnscaled(Program.formMain.ilPlayerAvatars.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatars, player.ImageIndexAvatar, player.IsLive)], pointIconAvatar);

            // Уровень Замка
            lblLevelCastle.Text = player.LevelCastle.ToString();

            // Прочность замка
            GuiUtils.DrawBand(e.Graphics, new Rectangle(Config.GRID_SIZE, Config.GRID_SIZE + Program.formMain.ilPlayerAvatars.ImageSize.Height + 1, Program.formMain.ilPlayerAvatars.ImageSize.Width, 4), brushCurDurability, brushMaxDurability, player.DurabilityCastle, player.Lobby.TypeLobby.DurabilityCastle);
            
            // Результат последнего боя
            if (player.ResultLastBattle != ResultBattle.None)
                e.Graphics.DrawImageUnscaled(Program.formMain.ilResultBattle.Images[(int)player.ResultLastBattle], pointIconResultBattle);

            if (player.LastBattleDamageToCastle != 0)
            {
                lblDamageToCastle.Show();
                int dmg = player.LastBattleDamageToCastle <= 999 ? player.LastBattleDamageToCastle : Math.Sign(player.LastBattleDamageToCastle) * 999;
                lblDamageToCastle.Text = dmg.ToString();
                lblDamageToCastle.ForeColor = player.LastBattleDamageToCastle > 0 ? Color.Green : Color.Red;
            }
            else
            {
                lblDamageToCastle.Hide();
            }

            // Указываем страйк
            if (player.ResultLastBattle != ResultBattle.None)
            {
                e.Graphics.DrawImageUnscaled(Program.formMain.ilGui24.Images[FormMain.GUI_24_FIRE], pointIconStrike);

                lblStrike.Show();
                lblStrike.Text = player.Strike.ToString();
                lblStrike.ForeColor = lblDamageToCastle.ForeColor;
            }
            else
                lblStrike.Hide();

            // Количество героев
            if (player.QuantityHeroes > 0)
            {
                lblQuantityHeroes.Show();
                e.Graphics.DrawImageUnscaled(Program.formMain.ilGui24.Images[FormMain.GUI_24_HEROES], pointIconHeroes);
                lblQuantityHeroes.Text = player.QuantityHeroes > 0 ? player.QuantityHeroes.ToString() : "";
            }
            else

                lblQuantityHeroes.Hide();
        }
    }
}
