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
        private Player player;
        private Pen penBorder = new Pen(Color.Black);

        public PanelPlayer(Player p) : base()
        {
            player = p;
            player.Panel = this;

            //BackColor = Color.LightBlue;

            Width = Program.formMain.ilPlayerAvatars.ImageSize.Width + Config.GRID_SIZE * 2;
            Height = Program.formMain.ilPlayerAvatars.ImageSize.Height + Config.GRID_SIZE * 3;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // Рамка вокруг панели
            e.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);

            // Иконка героя
            e.Graphics.DrawImageUnscaled(Program.formMain.ilPlayerAvatars.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatars, player.ImageIndexAvatar, player.IsLive)], Config.GRID_SIZE, Config.GRID_SIZE);
        }
    }
}
