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
        private static Font fontLevel = new Font("Arial", 12, FontStyle.Bold);
        private readonly PictureBox pbBuilding;
        private readonly ImageList imageListBuilding;
        private readonly ImageList imageListGui;
        private readonly ImageList imageListGui16;
        private readonly ImageList imageListGuiHeroes;
        private readonly Button btnBuyOrUpgrade;
        private readonly Button btnHireHero;
        private readonly Label lblName;
        private readonly Label lblIncome;
        private readonly Label lblLevel;
        private PlayerBuilding building;
        private readonly Font fontLabel = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        private readonly Pen penBorder;
        private readonly Rectangle rectBorder;

        public PanelBuilding(Control parent, int left, int top, FormMain formMain)
        {
            Parent = parent;
            //BorderStyle = BorderStyle.FixedSingle;
            imageListBuilding = formMain.ilBuildings;
            imageListGui = formMain.ilGui;
            imageListGui16 = formMain.ilGui16;
            imageListGuiHeroes = formMain.ilGuiHeroes;
            Left = left;
            Top = top;
            BackColor = Color.PowderBlue;

            lblName = new Label()
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Height = Config.GRID_SIZE * 2,
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
            pbBuilding.MouseEnter += PbBuilding_MouseEnter;
            pbBuilding.MouseLeave += Control_MouseLeave;

            btnBuyOrUpgrade = new Button()
            {
                Parent = this,
                Left = GuiUtils.NextLeft(pbBuilding),
                Size = GuiUtils.SizeButtonWithImage(imageListGui),
                Top = pbBuilding.Top + pbBuilding.Height - imageListGui.ImageSize.Height - 8,
                ImageList = imageListGui,
                TextAlign = ContentAlignment.BottomCenter,
                Font = formMain.fontCost,
                ForeColor = Color.White,
                BackgroundImage = formMain.background
            };
            btnBuyOrUpgrade.Click += BtnBuyOrUprgade_Click;
            btnBuyOrUpgrade.MouseEnter += BtnBuyOrUpgrade_MouseEnter;
            btnBuyOrUpgrade.MouseLeave += Control_MouseLeave;

            btnHireHero = new Button()
            {
                Parent = this,
                Left = btnBuyOrUpgrade.Left,
                Top = btnBuyOrUpgrade.Top - btnBuyOrUpgrade.Height - Config.GRID_SIZE,
                Size = btnBuyOrUpgrade.Size,
                ImageList = imageListGuiHeroes,
                TextAlign = ContentAlignment.BottomCenter,
                Font = formMain.fontCost,
                ForeColor = Color.White
            };
            btnHireHero.Click += BtnHero_Click;
            btnHireHero.MouseEnter += BtnHireHero_MouseEnter;
            btnHireHero.MouseLeave += Control_MouseLeave;

            lblLevel = new Label()
            {
                Parent = this,
                AutoSize = false,
                Top = lblName.Top,
                Size = new Size(32, 16),
                Font = fontLevel,
                TextAlign = ContentAlignment.MiddleRight
            };
            lblLevel.BringToFront();

            lblIncome = new Label()
            {
                Parent = this,
                Top = GuiUtils.NextTop(pbBuilding),
                Left = pbBuilding.Left,
                TextAlign = ContentAlignment.MiddleRight,
                ImageAlign = ContentAlignment.MiddleLeft,
                ImageIndex = FormMain.GUI_16_GOLD,
                ImageList = imageListGui16
            };

            Height = lblIncome.Top + lblIncome.Height + (Config.GRID_SIZE * 2);
            Width = btnBuyOrUpgrade.Left + btnBuyOrUpgrade.Width + Config.GRID_SIZE;

            lblName.Width = Width - (Config.GRID_SIZE * 2) - 2;
            lblLevel.Left = Width - Config.GRID_SIZE - lblLevel.Width;

            penBorder = new Pen(Color.Black);
            rectBorder = new Rectangle(0, 0, Width - 1, Height - 1);

            Paint += PanelBuilding_Paint;
        }

        private void BtnHireHero_MouseEnter(object sender, EventArgs e)
        {
            ShowHintBtnHireHero();
        }

        private void ShowHintBtnHireHero()
        {
            Program.formMain.formHint.ShowHint(new Point(Program.formMain.Left + 10 + Left + btnHireHero.Left, Program.formMain.Top + 32 + Top + btnHireHero.Top + btnHireHero.Height),
                building.Building.TrainedHero.Name, "",
                building.Building.TrainedHero.Description,
                building.GetTextRequirementsHire(),
                building.Building.TrainedHero.Cost, building.Player.Gold >= building.Building.TrainedHero.Cost, 0);
        }

        private void PbBuilding_MouseEnter(object sender, EventArgs e)
        {
            Program.formMain.formHint.ShowHint(new Point(Program.formMain.Left + 10 + Left + pbBuilding.Left, Program.formMain.Top + 32 + Top + pbBuilding.Top + pbBuilding.Height),
                building.Building.Name,
                building.Level > 0 ? "Уровень " + building.Level.ToString() : "",
                building.Building.Description, null, 0, false, building.Income());
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            Program.formMain.formHint.HideHint();
        }

        private void BtnBuyOrUpgrade_MouseEnter(object sender, EventArgs e)
        {
            ShowHintBtnBuyOrUpgrade();
        }

        private void ShowHintBtnBuyOrUpgrade()
        {
            Program.formMain.formHint.ShowHint(new Point(Program.formMain.Left + 10 + Left + btnBuyOrUpgrade.Left, Program.formMain.Top + 32 + Top + btnBuyOrUpgrade.Top + btnBuyOrUpgrade.Height),
                building.Building.Name,
                (building.Level == 0 ? "Уровень 1" : (building.CanLevelUp() == true) ? "Улучшить строение" : ""),
                building.Level == 0 ? building.Building.Description : "", building.GetTextRequirements(), building.CostBuyOrUpgrade(), building.Player.Gold >= building.CostBuyOrUpgrade(), building.IncomeNextLevel());
        }

        private void PanelBuilding_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(penBorder, rectBorder);
        }

        private void BtnHero_Click(object sender, EventArgs e)
        {
            Debug.Assert(building.Level < building.Building.MaxLevel);

            if ((building.Level > 0) && (building.CanTrainHero() == true))
            {
                building.HireHero();
                ShowHintBtnHireHero();
                UpdateData();
                Program.formMain.ShowHeroes();
            }
        }

        private void BtnBuyOrUprgade_Click(object sender, EventArgs e)
        {
            if (building.Player.Gold >= building.CostBuyOrUpgrade())
            {
                building.BuyOrUpgrade();
                ShowHintBtnBuyOrUpgrade();
                Program.formMain.ShowGold();
                Program.formMain.ShowAllBuildings();
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
            lblName.Text = building.Building.Name;
            lblIncome.Text = "+" + building.Income().ToString();

            // Информация об уровне здания
            if ((building.Level > 0) && (building.Building.MaxLevel > 1))
            {
                lblLevel.Show();

                if (building.Level < building.Building.MaxLevel)
                {
                    lblLevel.Text = building.Level.ToString() + "/" + building.Building.MaxLevel.ToString();
                    lblLevel.ForeColor = Color.Black;
                }
                else
                {
                    lblLevel.Text = building.Level.ToString();
                    lblLevel.ForeColor = Color.Green;
                }
            }
            else
                lblLevel.Hide();

            if (building.Building.TrainedHero != null)
            {
                btnHireHero.Show();
                btnHireHero.ImageIndex = GuiUtils.GetImageIndexWithGray(btnHireHero.ImageList, building.Building.TrainedHero.ImageIndex, building.CanTrainHero());
                btnHireHero.Text = (building.Level == 0) || (building.CanTrainHero() == true) ? building.Building.TrainedHero.Cost.ToString() : ""; //building.Heroes.Count.ToString() + "/" + building.Building.MaxHeroes.ToString();
            }
            else
                btnHireHero.Hide();

            if (building.Level > 0)
            {

                lblName.ForeColor = Color.Green;

                btnBuyOrUpgrade.Visible = building.CanLevelUp();
                if (building.CanLevelUp() == true)
                    btnBuyOrUpgrade.ImageIndex = GuiUtils.GetImageIndexWithGray(btnBuyOrUpgrade.ImageList, FormMain.GUI_LEVELUP, building.CheckRequirements());
                    
                //if (btnLevelUp.Visible == true)
                    //btnLevelUp.Image = building.CheckRequirements() == true ? imageListGui.Images[FormMain.GUI_LEVELUP] : imageListGui.Images[FormMain.GUI_LEVELUP + imageListGui.Images.Count / 2];
                pbBuilding.Image = imageListBuilding.Images[building.Building.ImageIndex];
            }
            else
            {
                lblName.ForeColor = Color.Gray;

                btnBuyOrUpgrade.Text = "";
                btnBuyOrUpgrade.ImageIndex = GuiUtils.GetImageIndexWithGray(btnBuyOrUpgrade.ImageList, FormMain.GUI_BUY, building.CheckRequirements());
                pbBuilding.Image = imageListBuilding.Images[building.Building.ImageIndex + FormMain.Config.Buildings.Count];
            }

            //if (btnBuyOrUpgrade.Visible == true)
            {
                btnBuyOrUpgrade.Text = building.CostBuyOrUpgrade().ToString();
            }
        }
    }
}