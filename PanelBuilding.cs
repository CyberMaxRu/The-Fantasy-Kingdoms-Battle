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
        private readonly ImageList imageListGuiHeroes;
        private readonly Button btnBuy;
        private readonly Button btnLevelUp;
        private readonly Label lblIncome;
        private readonly Label lblLevel;
        private readonly Label lblHeroes;
        private PlayerBuilding building;

        public PanelBuilding(int left, int top, ImageList ilBuilding, ImageList ilGui, ImageList ilGui16, ImageList ilGuiHeroes)
        {
            BorderStyle = BorderStyle.FixedSingle;
            imageListBuilding = ilBuilding;
            imageListGui = ilGui;
            imageListGuiHeroes = ilGuiHeroes;
            Left = left;
            Top = top;

            pbBuilding = new PictureBox()
            {
                Parent = this,
                Width = ilBuilding.ImageSize.Width + 2,// Окантовка
                Height = ilBuilding.ImageSize.Height + 2,// Окантовка
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
            };

            btnBuy = new Button()
            {
                Parent = this,
                Left = GuiUtils.NextLeft(pbBuilding),
                Size = GuiUtils.SizeButtonWithImage(ilGui),
                Top = pbBuilding.Top + pbBuilding.Height - ilGui.ImageSize.Height - 8,
                ImageList = imageListGui,
            };
            btnBuy.Click += BtnBuy_Click;

            btnLevelUp = new Button()
            {
                Parent = this,
                Left = btnBuy.Left,
                Top = btnBuy.Top - btnBuy.Height - Config.GRID_SIZE,
                Size = btnBuy.Size,
                ImageList = imageListGui,
                ImageIndex = FormMain.GUI_LEVELUP
            };
            btnLevelUp.Click += BtnLevelUp_Click;

            lblLevel = new Label()
            {
                Parent = this,
                Left = pbBuilding.Left,
                Top = pbBuilding.Top + pbBuilding.Height + Config.GRID_SIZE,
                Width = 80
            };

            lblHeroes = new Label()
            {
                Parent = this,
                Left = GuiUtils.NextLeft(lblLevel),
                Top = lblLevel.Top
            };

            lblIncome = new Label()
            {
                Parent = this,
                Top = lblLevel.Top,
                Left = GuiUtils.NextLeft(lblHeroes),
                TextAlign = ContentAlignment.MiddleRight,
                ImageAlign = ContentAlignment.MiddleLeft,
                ImageIndex = FormMain.GUI_16_GOLD,
                ImageList = ilGui16
            };

            Height = lblIncome.Top + lblIncome.Height + (Config.GRID_SIZE * 2);
            Width = btnBuy.Left + btnBuy.Width + Config.GRID_SIZE;
        }

        private void BtnLevelUp_Click(object sender, EventArgs e)
        {
            Debug.Assert(building.Level > 0);
            Debug.Assert(building.Level < building.Building.MaxLevel);

            building.BuyOrUpgrade();
            Program.formMain.ShowGold();
            Program.formMain.ShowAllBuildings();
        }

        private void BtnBuy_Click(object sender, EventArgs e)
        {
            if (building.Level == 0)
            {
                building.BuyOrUpgrade();
                UpdateData();
                Program.formMain.ShowGold();
            }
            else
            {
                building.HireHero();
                Program.formMain.ShowHeroes();
            }

            Program.formMain.ShowAllBuildings();
        }

        internal void ShowData(PlayerBuilding pb)
        {
            Debug.Assert(pb != null);

            building = pb;
            UpdateData();
        }

        internal void UpdateData()
        {
            lblIncome.Text = "+" + building.Income().ToString();

            if (building.Level > 0)
            {
                if (building.CanTrainHero() == true)
                {
                    btnBuy.Visible = true;
                    btnBuy.ImageList = imageListGuiHeroes;
                    btnBuy.ImageIndex = building.Building.TrainedHero.ImageIndex;
                }
                else
                {
                    btnBuy.Visible = false;
                }

                lblLevel.Text = building.Level.ToString();
                lblHeroes.Text = building.Heroes.Count.ToString() + "/" + building.Building.MaxHeroes.ToString();

                btnBuy.Text = building.Level.ToString();
                btnBuy.ImageIndex = -1;
                btnLevelUp.Visible = building.Level < building.Building.MaxLevel;
                if (btnLevelUp.Visible == true)
                    btnLevelUp.Image = building.CheckRequirements() == true ? imageListGui.Images[FormMain.GUI_LEVELUP] : imageListGui.Images[FormMain.GUI_LEVELUP + imageListGui.Images.Count / 2];
                pbBuilding.Image = imageListBuilding.Images[building.Building.ImageIndex];
            }
            else
            {
                lblLevel.Text = "";
                lblHeroes.Text = "";

                btnBuy.Text = "";
                btnBuy.ImageList = imageListGui;
                btnBuy.ImageIndex = FormMain.GUI_BUY;
                btnLevelUp.Visible = false;
                pbBuilding.Image = imageListBuilding.Images[building.Building.ImageIndex + FormMain.Config.Buildings.Count];
            }
        }
    }
}