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
        private readonly Button btnHeroes;
        private readonly ImageList imageListBuilding;
        private readonly ImageList imageListGui;
        private readonly ImageList imageListGui16;
        private readonly ImageList imageListGuiHeroes;
        private readonly Button btnBuyOrUpgrade;
        private readonly Button btnHireHero;
        private readonly Label lblName;
        private readonly Label lblIncome;
        private readonly Label lblLevel;
        private readonly Font fontLabel = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        private readonly Pen penBorder;
        private readonly Rectangle rectBorder;
        private readonly SolidBrush brushBackColor = new SolidBrush(Color.White);

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
            DoubleBuffered = true;

            lblName = new Label()
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Height = Config.GRID_SIZE * 2,
                BackColor = Color.Transparent,
                Font = fontLabel
            };

            pbBuilding = new PictureBox()
            {
                Parent = this,
                Width = imageListBuilding.ImageSize.Width + 2,// Окантовка
                Height = imageListBuilding.ImageSize.Height + 2,// Окантовка
                Left = Config.GRID_SIZE,
                Top = GuiUtils.NextTop(lblName),
                BackColor = Color.Transparent
            };
            pbBuilding.MouseEnter += Pbv_MouseEnter;
            pbBuilding.MouseLeave += Control_MouseLeave;
            pbBuilding.MouseClick += Pbv_MouseClick;

            btnHeroes = new Button()
            {
                Parent = this,
                ImageList = imageListGuiHeroes,
                Size = GuiUtils.SizeButtonWithImage(imageListGuiHeroes),
                Top = GuiUtils.NextTop(pbBuilding),
                Left = Config.GRID_SIZE,
                BackgroundImage = formMain.bmpForBackground,
                Font = formMain.fontCost,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.BottomCenter
            };

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
                BackgroundImage = formMain.bmpForBackground
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
                ForeColor = Color.White,
                BackgroundImage = formMain.bmpForBackground
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
                Left = btnBuyOrUpgrade.Left - Config.GRID_SIZE,
                Top = btnHeroes.Top,
                AutoSize = false,
                Width = btnBuyOrUpgrade.Width + Config.GRID_SIZE,
                Height = 20,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.Green,
                TextAlign = ContentAlignment.MiddleRight,
                ImageAlign = ContentAlignment.MiddleLeft,
                ImageIndex = FormMain.GUI_16_GOLD,
                ImageList = imageListGui16
            };

            Height = GuiUtils.NextTop(btnHeroes);// lblIncome. Top + lblIncome.Height + (Config.GRID_SIZE * 2);
            Width = btnBuyOrUpgrade.Left + btnBuyOrUpgrade.Width + Config.GRID_SIZE;

            lblName.Width = Width - (Config.GRID_SIZE * 2) - 2;
            lblLevel.Left = Width - Config.GRID_SIZE - lblLevel.Width;

            penBorder = new Pen(Color.Black);
            rectBorder = new Rectangle(0, 0, Width - 1, Height - 1);

            MouseClick += PanelBuilding_MouseClick;
        }

        internal PlayerBuilding Building { get; set; }

        private void SelectThisBuilding()
        {
            Program.formMain.SelectBuilding(this);
        }
        private void PanelBuilding_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SelectThisBuilding();
        }

        private void Pbv_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SelectThisBuilding();
        }

        private void BtnHireHero_MouseEnter(object sender, EventArgs e)
        {
            ShowHintBtnHireHero();
        }

        private void ShowHintBtnHireHero()
        {
            Program.formMain.formHint.ShowHint(new Point(10 + Left + btnHireHero.Left, Top + btnHireHero.Top + btnHireHero.Height),
                Building.Building.TrainedHero.Name, "",
                Building.Building.TrainedHero.Description,
                Building.GetTextRequirementsHire(),
                Building.Building.TrainedHero.Cost, Building.Player.Gold >= Building.Building.TrainedHero.Cost, 0, 0, false, null);
        }

        private void Pbv_MouseEnter(object sender, EventArgs e)
        {
            Program.formMain.formHint.ShowHint(new Point(10 + Left + pbBuilding.Left, Top + pbBuilding.Top + pbBuilding.Height),
                Building.Building.Name,
                Building.Level > 0 ? "Уровень " + Building.Level.ToString() : "",
                Building.Building.Description
                    + ((Building.Level > 0) && (Building.Building.TrainedHero != null) ? Environment.NewLine + Environment.NewLine + "Героев: " + Building.Heroes.Count.ToString() + "/" + Building.MaxHeroes().ToString() : "")
                , null, 0, false, Building.Income(), 0, false, null);
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
            if (Building.Level < Building.Building.MaxLevel)
                Program.formMain.formHint.ShowHint(new Point(10 + Left + btnBuyOrUpgrade.Left, Top + btnBuyOrUpgrade.Top + btnBuyOrUpgrade.Height),
                    Building.Building.Name,
                    Building.Level == 0 ? "Уровень 1" : (Building.CanLevelUp() == true) ? "Улучшить строение" : "",
                    Building.Level == 0 ? Building.Building.Description : "", Building.GetTextRequirements(), Building.CostBuyOrUpgrade(), Building.Player.Gold >= Building.CostBuyOrUpgrade(), Building.IncomeNextLevel(), Building.Builders(), Building.Player.FreeBuilders >= Building.Builders(), null);
            else
                Program.formMain.formHint.HideHint();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            brushBackColor.Color = Program.formMain.SelectedPanelBuilding == this ? Color.SkyBlue : BackColor = Color.PowderBlue;
            e.Graphics.FillRectangle(brushBackColor, ClientRectangle);

            e.Graphics.DrawRectangle(penBorder, rectBorder);

            if (Program.formMain.SelectedPanelBuilding == this)
                e.Graphics.DrawRectangle(penBorder, rectBorder.X + 1, rectBorder.Y + 1, rectBorder.Width - 2, rectBorder.Height - 2);
        }

        private void BtnHero_Click(object sender, EventArgs e)
        {
            Debug.Assert(Building.Level <= Building.Building.MaxLevel);

            SelectThisBuilding();

            if ((Building.Level > 0) && (Building.CanTrainHero() == true))
            {
                Building.HireHero();
                ShowHintBtnHireHero();
                UpdateData();
                Program.formMain.ShowPageHeroes();
            }
        }

        private void BtnBuyOrUprgade_Click(object sender, EventArgs e)
        {
            SelectThisBuilding();

            if (Building.Player.Gold >= Building.CostBuyOrUpgrade())
            {
                Building.BuyOrUpgrade();
                ShowHintBtnBuyOrUpgrade();
                Program.formMain.ShowGold();
                Program.formMain.ShowAllBuildings();
            }
        }

        internal void ShowData(PlayerBuilding pb)
        {
            Debug.Assert(pb != null);

            Building = pb;
            UpdateData();
        }

        internal void UpdateData()
        {
            lblName.Text = Building.Building.Name;
            lblIncome.ImageIndex = Building.DoIncome() == true ? FormMain.GUI_16_GOLD : -1;
            lblIncome.Text = Building.DoIncome() == true ? "+" + Building.Income().ToString() : "";
            lblIncome.ForeColor = Building.Level > 0 ? Color.Green : Color.Gray;

            // Информация об уровне здания
            if ((Building.Level > 0) && (Building.Building.MaxLevel > 1))
            {
                lblLevel.Show();

                if (Building.Level < Building.Building.MaxLevel)
                {
                    lblLevel.Text = Building.Level.ToString() + "/" + Building.Building.MaxLevel.ToString();
                    lblLevel.ForeColor = Color.Black;
                }
                else
                {
                    lblLevel.Text = Building.Level.ToString();
                    lblLevel.ForeColor = Color.Green;
                }
            }
            else
                lblLevel.Hide();

            if (Building.Building.TrainedHero != null)
            {
                btnHireHero.Show();
                btnHireHero.ImageIndex = (Building.Level > 0) && ((Building.Heroes.Count == Building.MaxHeroes()) || (Building.MaxHeroesAtPlayer() == true))  ? -1 : GuiUtils.GetImageIndexWithGray(btnHireHero.ImageList, Building.Building.TrainedHero.ImageIndex, Building.CanTrainHero());
                btnHireHero.Text = (Building.Level == 0) || (Building.CanTrainHero() == true) ? Building.Building.TrainedHero.Cost.ToString() : ""; 
            }
            else
                btnHireHero.Hide();

            if (Building.Level > 0)
            {

                lblName.ForeColor = Color.Green;

                if (Building.CanLevelUp() == true)
                {
                    btnBuyOrUpgrade.Text = Building.CostBuyOrUpgrade().ToString();
                    btnBuyOrUpgrade.ImageIndex = GuiUtils.GetImageIndexWithGray(btnBuyOrUpgrade.ImageList, FormMain.GUI_LEVELUP, Building.CheckRequirements());
                }
                else
                {
                    btnBuyOrUpgrade.Text = "";
                    btnBuyOrUpgrade.ImageIndex = -1;
                }

                //if (btnLevelUp.Visible == true)
                //btnLevelUp.Image = Building.CheckRequirements() == true ? imageListGui.Images[FormMain.GUI_LEVELUP] : imageListGui.Images[FormMain.GUI_LEVELUP + imageListGui.Images.Count / 2];
                pbBuilding.Image = imageListBuilding.Images[Building.Building.ImageIndex];
            }
            else
            {
                lblName.ForeColor = Color.Gray;

                btnBuyOrUpgrade.Text = Building.CostBuyOrUpgrade().ToString();
                btnBuyOrUpgrade.ImageIndex = GuiUtils.GetImageIndexWithGray(btnBuyOrUpgrade.ImageList, FormMain.GUI_BUY, Building.CheckRequirements());
                pbBuilding.Image = imageListBuilding.Images[Building.Building.ImageIndex + FormMain.Config.Buildings.Count];
            }

            if ((Building.Building.TrainedHero != null) && (Building.Level > 0))
            {
                btnHeroes.Text = Building.Heroes.Count.ToString();
                btnHeroes.ImageIndex = GuiUtils.GetImageIndexWithGray(btnHeroes.ImageList, Building.Building.TrainedHero.ImageIndex, true);
            }
            else
            {
                btnHeroes.Text = "";
                btnHeroes.ImageIndex = -1;
            }
        }
    }
}