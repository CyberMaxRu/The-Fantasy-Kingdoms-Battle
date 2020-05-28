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
        private readonly int LeftForResultBattle;
        private readonly Pen penBorder = new Pen(Color.Black);
        private readonly SolidBrush brushCurDurability = new SolidBrush(Color.Green);
        private readonly SolidBrush brushMaxDurability = new SolidBrush(Color.LightGreen);

        public PanelPlayer(Player p) : base()
        {
            player = p;
            player.Panel = this;

            Width = Config.GRID_SIZE + Program.formMain.ilPlayerAvatars.ImageSize.Width + Config.GRID_SIZE + Program.formMain.ilResultBattle.ImageSize.Width + Config.GRID_SIZE;
            Height = Config.GRID_SIZE + Program.formMain.ilPlayerAvatars.ImageSize.Height + Config.GRID_SIZE + Config.GRID_SIZE;

            LeftForResultBattle = Width - Program.formMain.ilResultBattle.ImageSize.Width - Config.GRID_SIZE;

            lblDamageToCastle = new Label()
            {
                Parent = this,
                Left = LeftForResultBattle - Config.GRID_SIZE_HALF,
                Top = Config.GRID_SIZE + Program.formMain.ilResultBattle.ImageSize.Height + Config.GRID_SIZE_HALF,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.TopCenter,
                MaximumSize = new Size(Program.formMain.ilResultBattle.ImageSize.Width + Config.GRID_SIZE, Config.GRID_SIZE * 2),
                Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold)
            };

            lblStrike = new Label()
            {
                Parent = this,
                Left = LeftForResultBattle,
                Top = Config.GRID_SIZE,
                ForeColor = Color.Black,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                MaximumSize = Program.formMain.ilResultBattle.ImageSize,
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold)
            };

            lblQuantityHeroes = new Label()
            {
                Parent = this,
                Left = LeftForResultBattle,
                Top = Height - Config.GRID_SIZE * 2 - Config.GRID_SIZE_HALF,
                ForeColor = Color.Black,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.BottomCenter,
                MaximumSize = new Size(Program.formMain.ilResultBattle.ImageSize.Width, Config.GRID_SIZE * 2),
                Font = new Font("Microsoft Sans Serif", 9)
            };
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Program.formMain.formHint.ShowHint(new Point(Left, Top + Height + 2),
                player.Name,
                "Место №" + player.PositionInLobby.ToString(),
                "Прочность замка " + player.DurabilityCastle.ToString() + "/" + player.Lobby.TypeLobby.DurabilityCastle.ToString()
                    + Environment.NewLine + "Героев: " + player.QuantityHeroes.ToString(),
                null,
                0,
                false, 0,
                0, false, null);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Program.formMain.formHint.HideHint();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (player == player.Lobby.CurrentPlayer)
                BackColor = Color.LightBlue;
            else if (player == player.Lobby.CurrentPlayer.Opponent)
                BackColor = Color.LightCoral;
            else
                BackColor = Color.FromKnownColor(KnownColor.Control);

            // Рамка вокруг панели
            e.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);

            // Иконка героя
            e.Graphics.DrawImageUnscaled(Program.formMain.ilPlayerAvatars.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatars, player.ImageIndexAvatar, player.IsLive)], Config.GRID_SIZE, Config.GRID_SIZE);

            // Прочность замка
            GuiUtils.DrawBand(e.Graphics, new Rectangle(Config.GRID_SIZE, Config.GRID_SIZE + Program.formMain.ilPlayerAvatars.ImageSize.Height + 1, Program.formMain.ilPlayerAvatars.ImageSize.Width, 4), brushCurDurability, brushMaxDurability, player.DurabilityCastle, player.Lobby.TypeLobby.DurabilityCastle);

            // Результат последнего боя
            if (player.ResultLastBattle != ResultBattle.None)
                e.Graphics.DrawImageUnscaled(Program.formMain.ilResultBattle.Images[(int)player.ResultLastBattle], LeftForResultBattle, Config.GRID_SIZE);

            // Указываем страйк, если он есть
            if (player.Strike > 1)
            {
                lblStrike.Show();
                lblStrike.Text = player.Strike.ToString();
            }
            else
                lblStrike.Hide();

            // Урон по Замку в последнем бою
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

            // Количество героев
            lblQuantityHeroes.Text = player.QuantityHeroes.ToString();
        }
    }
}
