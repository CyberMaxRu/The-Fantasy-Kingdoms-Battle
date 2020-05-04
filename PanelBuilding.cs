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
    internal sealed class PanelBuilding : Control
    {
        private readonly PictureBox pbBuilding;
        private readonly ImageList imageListBuilding;
        private readonly ImageList imageListGui;
        private readonly ImageList imageListGuiHeroes;
        private readonly Button btnBuy;
        private readonly Button btnLevelUp;
        private readonly Label lblName;
        private readonly Label lblIncome;
        private readonly Label lblLevel;
        private readonly Label lblHeroes;
        private PlayerBuilding building;
        private readonly Font fontLabel = new Font("Microsoft Sans Setif", 10, FontStyle.Bold);

        public PanelBuilding(Control parent, int left, int top, FormMain formMain)
        {
            Parent = parent;
            //BorderStyle = BorderStyle.FixedSingle;
            imageListBuilding = formMain.ilBuildings;
            imageListGui = formMain.ilGui;
            imageListGuiHeroes = formMain.ilGuiHeroes;
            Left = left;
            Top = top;

            lblName = new Label()
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Height = Config.GRID_SIZE * 2,
                ForeColor = Color.FromKnownColor(KnownColor.Green),
                Font = fontLabel
            };

            pbBuilding = new PictureBox()
            {
                Parent = this,
                Width = imageListBuilding.ImageSize.Width + 2,// Окантовка
                Height = imageListBuilding.ImageSize.Height + 2,// Окантовка
                Left = Config.GRID_SIZE,
                Top = GuiUtils.NextTop(lblName)
            };

            btnBuy = new Button()
            {
                Parent = this,
                Left = GuiUtils.NextLeft(pbBuilding),
                Size = GuiUtils.SizeButtonWithImage(imageListGui),
                Top = pbBuilding.Top + pbBuilding.Height - imageListGui.ImageSize.Height - 8,
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
                ImageList = imageListGui
            };

            Height = lblIncome.Top + lblIncome.Height + (Config.GRID_SIZE * 2);
            Width = btnBuy.Left + btnBuy.Width + Config.GRID_SIZE;

            lblName.Width = Width - (Config.GRID_SIZE * 2) - 2;
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
            lblName.Text = building.Building.Name;
            lblIncome.Text = "+" + building.Income().ToString();

            if (building.Level > 0)
            {
                if ((building.Building.MaxHeroes > 0) && (building.CanTrainHero() == true))
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