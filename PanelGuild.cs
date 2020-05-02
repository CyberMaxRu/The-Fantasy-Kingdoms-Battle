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
        private readonly Label lblLevel;
        private readonly Label lblHeroes;
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

            lblLevel = new Label()
            {
                Parent = this,
                Left = pbGuild.Left,
                Top = pbGuild.Top + pbGuild.Height + Config.GRID_SIZE,
                Width = 80
            };

            lblHeroes = new Label()
            {
                Parent = this,
                Left = lblLevel.Left + lblLevel.Width + Config.GRID_SIZE,
                Top = lblLevel.Top
            };

            Height = lblLevel.Top + lblLevel.Height + Config.GRID_SIZE;
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
                guild.HireHero();
                Program.formMain.ShowHeroes();
            }

            Program.formMain.ShowAllBuildings();
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
                if (guild.CanTrainHero() == true)
                {
                    btnBuy.Visible = true;
                    btnBuy.ImageList = imageListGuiHeroes;
                    btnBuy.ImageIndex = guild.Guild.TrainedHero.ImageIndex;
                }
                else
                {
                    btnBuy.Visible = false;
                }
                btnLevelUp.Visible = true;
                pbGuild.Image = imageListGuild.Images[guild.Guild.ImageIndex];
                lblLevel.Text = guild.Level.ToString();
                lblHeroes.Text = guild.Heroes.Count.ToString() + "/" + guild.Guild.MaxHeroes.ToString();
            }
            else
            {
                btnBuy.ImageList = imageListGui;
                btnBuy.ImageIndex = FormMain.GUI_BUY;
                btnLevelUp.Visible = false;
                pbGuild.Image = imageListGuild.Images[guild.Guild.ImageIndex + FormMain.Config.Guilds.Count];
                lblLevel.Text = "";
                lblHeroes.Text = "";
            }
        }
    }
}
