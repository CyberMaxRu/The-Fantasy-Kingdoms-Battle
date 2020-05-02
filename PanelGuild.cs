using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс панели гильдии
    internal sealed class PanelGuild : Panel
    {
        private readonly PictureBox pbGuild;
        private readonly ImageList imageListGuild;
        private readonly ImageList imageListGui;
        private readonly ImageList imageListGuiHeroes;
        private readonly Button btnBuy;
        private readonly Button btnLevelUp;
        private PlayerGuild guild;

        public PanelGuild(int left, int top,  ImageList ilGuild, ImageList ilGui, ImageList ilGuiHeroes)
        {
            BorderStyle = BorderStyle.FixedSingle;
            imageListGuild = ilGuild;
            imageListGui = ilGui;
            imageListGuiHeroes = ilGuiHeroes;
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

            btnBuy = new Button()
            {
                Parent = this,
                Left = pbGuild.Left + pbGuild.Width + Config.GRID_SIZE,
                Width = ilGui.ImageSize.Width + 8,
                Height = ilGui.ImageSize.Height + 8,
                Top = pbGuild.Top + pbGuild.Height - ilGui.ImageSize.Height - 8,
                ImageList = imageListGui,
            };
            btnBuy.Click += BtnBuy_Click;

            btnLevelUp = new Button()
            {
                Parent = this,
                Left = btnBuy.Left,
                Top = btnBuy.Top - btnBuy.Height - Config.GRID_SIZE,
                Width = btnBuy.Width,
                Height = btnBuy.Height,
                ImageList = imageListGui,
                ImageIndex = FormMain.GUI_LEVELUP
            };

            Height = pbGuild.Height + (Config.GRID_SIZE * 2);
            Width = btnBuy.Left + btnBuy.Width + Config.GRID_SIZE;
        }

        private void BtnBuy_Click(object sender, EventArgs e)
        {
            if (guild.Level == 0)
            {
                guild.Buy();
                UpdateData();
                Program.formMain.ShowGold();
            }
            else
            {

            }
        }
    
        internal void ShowData(PlayerGuild pg)
        {
            Debug.Assert(pg != null);

            guild = pg;
            UpdateData();
        }

        internal void UpdateData()
        {
            if (guild.Level > 0)
            {
                btnBuy.ImageList = imageListGuiHeroes;
                btnBuy.ImageIndex = guild.Guild.TrainedHero.ImageIndex;
                btnLevelUp.Visible = true;
                pbGuild.Image = imageListGuild.Images[guild.Guild.ImageIndex];
            }
            else
            {
                btnBuy.ImageList = imageListGui;
                btnBuy.ImageIndex = FormMain.GUI_BUY;
                btnLevelUp.Visible = false;
                pbGuild.Image = imageListGuild.Images[guild.Guild.ImageIndex + FormMain.Config.Guilds.Count];
            }
        }
    }
}
