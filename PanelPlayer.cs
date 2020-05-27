using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Класс панели игрока (сокращенная информация)
    internal sealed class PanelPlayer : Control
    {
        private PictureBox pbPlayer;
        private Player player;

        public PanelPlayer(Player p) : base()
        {
            player = p;
            player.Panel = this;

            pbPlayer = new PictureBox()
            {
                Parent = this,
                Width = Program.formMain.ilPlayerAvatars.ImageSize.Width,
                Height = Program.formMain.ilPlayerAvatars.ImageSize.Height,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
            };

            Width = pbPlayer.Width + Config.GRID_SIZE * 2;
            Height = pbPlayer.Height + Config.GRID_SIZE * 3;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            pbPlayer.Image = Program.formMain.ilPlayerAvatars.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatars, player.ImageIndexAvatar, player.IsLive)];
        }
    }
}
