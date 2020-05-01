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
    // Класс панели здания
    internal sealed class PanelBuilding : Panel
    {
        private readonly PictureBox pbBuilding;
        private readonly ImageList imageListBuilding;
        private readonly ImageList imageListGui;
        private readonly Button btnBuy;
        private readonly Button btnLevelUp;
        private PlayerBuilding building;

        public PanelBuilding(int left, int top, ImageList ilBuilding, ImageList ilGui)
        {
            BorderStyle = BorderStyle.FixedSingle;
            imageListBuilding = ilBuilding;
            imageListGui = ilGui;
            Left = left;
            Top = top;

            pbBuilding = new PictureBox()
            {
                Parent = this,
                Width = ilBuilding.ImageSize.Width,
                Height = ilBuilding.ImageSize.Height,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
            };

            btnBuy = new Button()
            {
                Parent = this,
                Left = pbBuilding.Left + pbBuilding.Width + Config.GRID_SIZE,
                Width = ilGui.ImageSize.Width + 8,
                Height = ilGui.ImageSize.Height + 8,
                Top = pbBuilding.Top + pbBuilding.Height - ilGui.ImageSize.Height - 8,
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

            Height = pbBuilding.Height + (Config.GRID_SIZE * 2);
            Width = btnBuy.Left + btnBuy.Width + Config.GRID_SIZE;
        }

        private void BtnBuy_Click(object sender, EventArgs e)
        {
            if (building.Level == 0)
            {
                building.Buy();
                UpdateData();
            }
        }

        internal void ShowData(PlayerBuilding pb)
        {
            Debug.Assert(pb != null);

            building = pb;
            UpdateData();
        }

        internal void UpdateData()
        {
            if (building.Level > 0)
            {
                btnBuy.Text = building.Level.ToString();
                btnBuy.ImageIndex = -1;
                btnLevelUp.Visible = building.Level < building.Building.MaxLevel;
                pbBuilding.Image = imageListBuilding.Images[building.Building.ImageIndex];
            }
            else
            {
                btnBuy.Text = "";
                btnBuy.ImageIndex = FormMain.GUI_BUY;
                btnLevelUp.Visible = false;
                pbBuilding.Image = imageListBuilding.Images[building.Building.ImageIndex + FormMain.Config.Buildings.Count];
            }
        }
    }
}