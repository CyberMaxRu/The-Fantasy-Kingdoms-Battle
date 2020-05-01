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
    // Класс панели храма
    internal sealed class PanelTemple : Panel
    {
        private readonly PictureBox pTemple;
        private readonly ImageList imageListTemple;
        private readonly ImageList imageListGui;
        private readonly Button btnBuy;
        private readonly Button btnLevelUp;
        private PlayerTemple temple;

        public PanelTemple(int left, int top, ImageList ilTemple, ImageList ilGui)
        {
            BorderStyle = BorderStyle.FixedSingle;
            imageListTemple = ilTemple;
            imageListGui = ilGui;
            Left = left;
            Top = top;

            pTemple = new PictureBox()
            {
                Parent = this,
                Width = imageListTemple.ImageSize.Width,
                Height = imageListTemple.ImageSize.Height,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
            };

            btnBuy = new Button()
            {
                Parent = this,
                Left = pTemple.Left + pTemple.Width + Config.GRID_SIZE,
                Width = ilGui.ImageSize.Width + 8,
                Height = ilGui.ImageSize.Height + 8,
                Top = pTemple.Top + pTemple.Height - ilGui.ImageSize.Height - 8,
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

            Height = pTemple.Height + (Config.GRID_SIZE * 2);
            Width = btnBuy.Left + btnBuy.Width + Config.GRID_SIZE;
        }

        private void BtnBuy_Click(object sender, EventArgs e)
        {
            if (temple.Level == 0)
            {
                temple.Buy();
                UpdateData();
                Program.formMain.ShowGold();
            }
        }

        internal void ShowData(PlayerTemple pt)
        {
            Debug.Assert(pt != null);

            temple = pt;
            UpdateData();
        }

        internal void UpdateData()
        {
            if (temple.Level > 0)
            {
                btnBuy.Text = temple.Level.ToString();
                btnBuy.ImageIndex = -1;
                btnLevelUp.Visible = true;
                pTemple.Image = imageListTemple.Images[temple.Temple.ImageIndex];
            }
            else
            {
                btnBuy.Text = "";
                btnBuy.ImageIndex = FormMain.GUI_BUY;
                btnLevelUp.Visible = false;
                pTemple.Image = imageListTemple.Images[temple.Temple.ImageIndex + FormMain.Config.Temples.Count];
            }
        }
    }
}
