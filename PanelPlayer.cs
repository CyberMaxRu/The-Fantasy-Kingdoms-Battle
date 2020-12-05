using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс панели игрока (сокращенная информация)
    internal sealed class PanelPlayer : BasePanel
    {
        private readonly Player player;
        private PanelEntity panelAvatar;
        private Label lblDamageToCastle;
        private Label lblStrike;
        private Label lblQuantityHeroes;
        private Rectangle rectBorder;
        private Point pointIconAvatar;// Координаты для отрисовки аватара игрока
        private Point pointIconResultBattle;// Координаты для иконки результата боя
        private Point pointIconStrike;// Координаты для иконки страйка
        private Point pointIconHeroes;// Координаты для иконки героев
        private readonly Pen penBorder = new Pen(FormMain.Config.CommonBorder);
        private readonly SolidBrush brushCurDurability = new SolidBrush(FormMain.Config.BattlefieldPlayerHealth);
        private readonly SolidBrush brushMaxDurability = new SolidBrush(FormMain.Config.BattlefieldPlayerHealthNone);
        private readonly SolidBrush brushBackground = new SolidBrush(Color.White);

        public PanelPlayer(Player p, Control parent) : base()
        {
            player = p;
            player.Panel = this;

            Parent = parent;
            Left = FormMain.Config.GridSize;

            panelAvatar = new PanelEntity()
            {
                Parent = this,
                Location = new Point(FormMain.Config.GridSize, FormMain.Config.GridSize),
                ShowHint = false
            };
            panelAvatar.ShowCell(player);
            panelAvatar.MouseEnter += PanelAvatar_MouseEnter;
            panelAvatar.MouseLeave += PanelAvatar_MouseLeave;

            pointIconAvatar = new Point(FormMain.Config.GridSize, FormMain.Config.GridSize);

            int leftForIcons = pointIconAvatar.X + Program.formMain.ilGuiHeroes.ImageSize.Width + FormMain.Config.GridSize;
            int leftForText = leftForIcons + Program.formMain.ilGui24.ImageSize.Width + FormMain.Config.GridSizeHalf;

            pointIconResultBattle = new Point(leftForIcons, pointIconAvatar.Y);
            pointIconStrike = new Point(leftForIcons, pointIconResultBattle.Y + Program.formMain.ilResultBattle.ImageSize.Height + FormMain.Config.GridSizeHalf);
            pointIconHeroes = new Point(leftForIcons, pointIconStrike.Y + Program.formMain.ilGui24.ImageSize.Height + FormMain.Config.GridSizeHalf);

            lblDamageToCastle = new Label()
            {
                Parent = this,
                Left = leftForText,
                Top = pointIconResultBattle.Y,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                MaximumSize = new Size(Program.formMain.ilResultBattle.ImageSize.Width + FormMain.Config.GridSize, Program.formMain.ilResultBattle.ImageSize.Height),
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
                ForeColor = FormMain.Config.CommonBorder,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                MaximumSize = Program.formMain.ilGui24.ImageSize,
                Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold)
            };

            Width = leftForText + lblDamageToCastle.MaximumSize.Width + FormMain.Config.GridSizeHalf;
            Height = Math.Max(FormMain.Config.GridSize + Program.formMain.ilPlayerAvatars.ImageSize.Height + FormMain.Config.GridSize + FormMain.Config.GridSize,
                GuiUtils.NextTop(lblQuantityHeroes));

            rectBorder = new Rectangle(0, 0, Width - 1, Height - 1);
        }

        private void PanelAvatar_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void PanelAvatar_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            Program.formMain.formHint.Clear();
            (player as ICell).PrepareHint();
            Program.formMain.formHint.ShowHint(this);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Program.formMain.formHint.HideHint();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Debug.Assert(player.LastBattleDamageToCastle >= -999);
            Debug.Assert(player.LastBattleDamageToCastle <= 999);

            // Фон панели
            if (player == player.Lobby.CurrentPlayer)
                brushBackground.Color = Color.LightBlue;
            else if (player == player.Lobby.CurrentPlayer.Opponent)
                brushBackground.Color = Color.LightCoral;
            else
                brushBackground.Color = Color.FromKnownColor(KnownColor.Control);

            //e.Graphics.FillRectangle(brushBackground, rectBorder);

            // Рамка вокруг панели
            e.Graphics.DrawRectangle(penBorder, rectBorder);

            // Прочность замка
            int dur = player.DurabilityCastle >= 0 ? player.DurabilityCastle : 0;
            GuiUtils.DrawBand(e.Graphics, new Rectangle(FormMain.Config.GridSize, FormMain.Config.GridSize + panelAvatar.Height + 1, panelAvatar.Width, 6), brushCurDurability, brushMaxDurability, dur, player.Lobby.TypeLobby.DurabilityCastle);
            
            // Результат последнего боя
            if (player.ResultLastBattle != ResultBattle.None)
                e.Graphics.DrawImageUnscaled(Program.formMain.ilResultBattle.Images[(int)player.ResultLastBattle], pointIconResultBattle);

            if (player.LastBattleDamageToCastle != 0)
            {
                lblDamageToCastle.Show();
                lblDamageToCastle.Text = player.LastBattleDamageToCastle.ToString();
                lblDamageToCastle.ForeColor = player.LastBattleDamageToCastle > 0 ? FormMain.Config.DamageToCastlePositive : FormMain.Config.DamageToCastleNegative;
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
                lblStrike.Text = player.Streak.ToString();
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
