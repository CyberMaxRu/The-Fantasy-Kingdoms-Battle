using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс панели гильдии
    internal sealed class PanelGuild : Panel
    {
        private readonly PictureBox pbGuild;
        private readonly ImageList imageListGuild;
        public PanelGuild(int left, int top,  ImageList ilGuild)
        {
            BorderStyle = BorderStyle.FixedSingle;
            imageListGuild = ilGuild;
            Left = left;
            Top = top;

            pbGuild = new PictureBox()
            {
                Parent = this,
                Width = ilGuild.ImageSize.Width,
                Height = ilGuild.ImageSize.Height,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
            };

            Height = pbGuild.Height + (Config.GRID_SIZE * 2);
            Width = pbGuild.Left + pbGuild.Width + Config.GRID_SIZE;
        }

        internal void ShowData(PlayerGuild pg)
        {
            if (pg.Level > 0)
                pbGuild.Image = imageListGuild.Images[pg.Guild.ImageIndex];
            else
                pbGuild.Image = imageListGuild.Images[pg.Guild.ImageIndex + FormMain.Config.Guilds.Count];
        }
    }
}
