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
        private readonly Pen penBorder = new Pen(Color.Black);
        private readonly SolidBrush brushCurDurability = new SolidBrush(Color.Green);
        private readonly SolidBrush brushMaxDurability = new SolidBrush(Color.LightGreen);

        public PanelPlayer(Player p) : base()
        {
            player = p;
            player.Panel = this;

            //BackColor = Color.LightBlue;

            Width = Program.formMain.ilPlayerAvatars.ImageSize.Width + Config.GRID_SIZE * 2;
            Height = Program.formMain.ilPlayerAvatars.ImageSize.Height + Config.GRID_SIZE * 3;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Program.formMain.formHint.ShowHint(new Point(Program.formMain.Left + Left, Program.formMain.Top + 32 + Top + Height + 2),
                player.Name,
                "",
                "Прочность замка " + player.DurabilityCastle.ToString() + "/" + player.Lobby.TypeLobby.DurabilityCastle.ToString(),
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
            // Рамка вокруг панели
            e.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);

            // Иконка героя
            e.Graphics.DrawImageUnscaled(Program.formMain.ilPlayerAvatars.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatars, player.ImageIndexAvatar, player.IsLive)], Config.GRID_SIZE, Config.GRID_SIZE);

            // Прочность замка
            GuiUtils.DrawBand(e.Graphics, new Rectangle(Config.GRID_SIZE, Config.GRID_SIZE + Program.formMain.ilPlayerAvatars.ImageSize.Height + 1, Program.formMain.ilPlayerAvatars.ImageSize.Width, 4), brushCurDurability, brushMaxDurability, player.DurabilityCastle, player.Lobby.TypeLobby.DurabilityCastle);
        }
    }
}
