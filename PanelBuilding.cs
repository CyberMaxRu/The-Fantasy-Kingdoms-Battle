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
    internal sealed class PanelBuilding : ControlContainer
    {
        private PlayerBuilding building;
        private readonly PictureBox pbBuilding;
        private readonly Button btnHeroes;
        private readonly Button btnBuyOrUpgrade;
        private readonly Button btnHireHero;
        private readonly Label lblName;
        private readonly Label lblIncome;
        private readonly Label lblLevel;
        private readonly Pen penBorder;
        private readonly Rectangle rectBorder;

        public PanelBuilding() : base()
        {
            lblName = new Label()
            {
                Height = FormMain.Config.GridSize * 2,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontBuildingCaption
            };
            AddControl(lblName, new Point(FormMain.Config.GridSize, FormMain.Config.GridSize));

            pbBuilding = new PictureBox()
            {
                Width = Program.formMain.ilBuildings.ImageSize.Width + 2,// Окантовка
                Height = Program.formMain.ilBuildings.ImageSize.Height + 2,// Окантовка
                BackColor = Color.Transparent
            };
            AddControl(pbBuilding, new Point(FormMain.Config.GridSize, GuiUtils.NextTop(lblName)));
            pbBuilding.MouseEnter += pbBuilding_MouseEnter;
            pbBuilding.MouseLeave += Control_MouseLeave;
            pbBuilding.MouseClick += pbBuilding_MouseClick;
            
            btnHeroes = new Button()
            {
                ImageList = Program.formMain.ilGuiHeroes,
                Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGuiHeroes),
                //BackgroundImage = Program.formMain.bmpBackgroundButton,
                Font = FormMain.Config.FontCost,
                ForeColor = FormMain.Config.CommonCost,
                TextAlign = ContentAlignment.BottomCenter
            };
            AddControl(btnHeroes, new Point(FormMain.Config.GridSize, GuiUtils.NextTop(pbBuilding)));

            btnBuyOrUpgrade = new Button()
            {
                Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGui),
                ImageList = Program.formMain.ilGui,
                TextAlign = ContentAlignment.BottomCenter,
                Font = FormMain.Config.FontCost,
                //BackgroundImage = Program.formMain.bmpBackgroundButton,
                ForeColor = FormMain.Config.CommonCost
            };
            AddControl(btnBuyOrUpgrade, new Point(GuiUtils.NextLeft(pbBuilding), pbBuilding.Top + pbBuilding.Height - Program.formMain.ilGui.ImageSize.Height - 8));
            btnBuyOrUpgrade.Click += BtnBuyOrUprgade_Click;
            btnBuyOrUpgrade.MouseEnter += BtnBuyOrUpgrade_MouseEnter;
            btnBuyOrUpgrade.MouseLeave += Control_MouseLeave;

            btnHireHero = new Button()
            {
                Size = btnBuyOrUpgrade.Size,
                ImageList = Program.formMain.ilGuiHeroes,
                TextAlign = ContentAlignment.BottomCenter,
                Font = FormMain.Config.FontCost,
                //BackgroundImage = Program.formMain.bmpBackgroundButton,
                ForeColor = Color.White
            };
            AddControl(btnHireHero, new Point(btnBuyOrUpgrade.Left, btnBuyOrUpgrade.Top - btnBuyOrUpgrade.Height - FormMain.Config.GridSize));
            btnHireHero.Click += BtnHero_Click;
            btnHireHero.MouseEnter += BtnHireHero_MouseEnter;
            btnHireHero.MouseLeave += Control_MouseLeave;

            lblLevel = new Label()
            {
                AutoSize = false,
                Size = new Size(32, 16),
                //BackColor = Color.Transparent,
                Font = FormMain.Config.FontBuildingLevel,
                TextAlign = ContentAlignment.MiddleRight
            };
            AddControl(lblLevel, new Point(0, lblName.Top));
            lblLevel.BringToFront();

            lblIncome = new Label()
            {
                AutoSize = false,
                Width = btnBuyOrUpgrade.Width + FormMain.Config.GridSize,
                Height = 20,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.Green,
                TextAlign = ContentAlignment.MiddleRight,
                ImageAlign = ContentAlignment.MiddleLeft,
                ImageIndex = FormMain.GUI_16_GOLD,
                ImageList = Program.formMain.ilGui16
            };
            AddControl(lblIncome, new Point(btnBuyOrUpgrade.Left, btnHeroes.Top));

            Height = GuiUtils.NextTop(btnHeroes);// lblIncome. Top + lblIncome.Height + (Config.GRID_SIZE * 2);
            Width = btnBuyOrUpgrade.Left + btnBuyOrUpgrade.Width + FormMain.Config.GridSize;

            lblName.Width = Width - (FormMain.Config.GridSize * 2) - 2;
            ArrangeControl(lblLevel, new Point(Width - FormMain.Config.GridSize - lblLevel.Width, lblLevel.Top));

            penBorder = new Pen(Color.Black);
            rectBorder = new Rectangle(0, 0, Width - 1, Height - 1);

            // Восстановить
            //MouseClick += PanelBuilding_MouseClick;
        }

        internal PlayerBuilding Building { get { return building; } set { Debug.Assert(value != null); building = value; UpdateData(); } }

        private void SelectThisBuilding()
        {
            Program.formMain.SelectBuilding(this);
        }
        private void PanelBuilding_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SelectThisBuilding();
        }

        private void pbBuilding_MouseClick(object sender, MouseEventArgs e)
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
            /*Program.formMain.formHint.Clear();
            Program.formMain.formHint.AddStep1Header(Building.Building.TrainedHero.Name, "", Building.Building.TrainedHero.Description);
            if ((Building.Building.TrainedHero != null) && (Building.Building.TrainedHero.Cost > 0))
                Program.formMain.formHint.AddStep3Requirement(Building.GetTextRequirementsHire());
            Program.formMain.formHint.AddStep4Gold(Building.Building.TrainedHero.Cost, Building.Player.Gold >= Building.Building.TrainedHero.Cost);
            Program.formMain.formHint.ShowHint(btnHireHero);
            */
        }

        private void pbBuilding_MouseEnter(object sender, EventArgs e)
        {
            /*Program.formMain.formHint.Clear();
            Program.formMain.formHint.AddStep1Header(Building.Building.Name, Building.Level > 0 ? "Уровень " + Building.Level.ToString() : "", Building.Building.Description + ((Building.Level > 0) && (Building.Building.TrainedHero != null) ? Environment.NewLine + Environment.NewLine + "Героев: " + Building.Heroes.Count.ToString() + "/" + Building.MaxHeroes().ToString() : ""));
            Program.formMain.formHint.AddStep2Income(Building.Income());
            Program.formMain.formHint.ShowHint(this);
            */
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
            //Debug.Assert(Building.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            if (Building.Level < Building.Building.MaxLevel)
            {
                Program.formMain.formHint.Clear();
                Program.formMain.formHint.AddStep1Header(Building.Building.Name, Building.Level == 0 ? "Уровень 1" : (Building.CanLevelUp() == true) ? "Улучшить строение" : "", Building.Level == 0 ? Building.Building.Description : "");
                Program.formMain.formHint.AddStep2Income(Building.IncomeNextLevel());
                Program.formMain.formHint.AddStep3Requirement(Building.GetTextRequirements());
                Program.formMain.formHint.AddStep4Gold(Building.CostBuyOrUpgrade(), Building.Player.Gold >= Building.CostBuyOrUpgrade());
                Program.formMain.formHint.AddStep5Builders(Building.Builders(), Building.Player.FreeBuilders >= Building.Builders());
                Program.formMain.formHint.ShowHint(btnBuyOrUpgrade);
            }
            else
                Program.formMain.formHint.HideHint();
        }

        private void BtnHero_Click(object sender, EventArgs e)
        {
            Debug.Assert(Building.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);
            Debug.Assert(Building.Level <= Building.Building.MaxLevel);

            // Восстановить
            /* SelectThisBuilding();

            if ((Building.Level > 0) && (Building.CanTrainHero() == true))
            {
                if (Building.Building.CategoryBuilding == CategoryBuilding.Temple)
                {
                    MessageBox.Show("Найм храмовых героев в этой версии не реализован.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Building.HireHero();
                ShowHintBtnHireHero();
                UpdateData();
                Program.formMain.ShowPageHeroes();
                Program.formMain.UpdateBuildingInfo();// Надо обновить список жителей гильдии
            }
            */
        }

        private void BtnBuyOrUprgade_Click(object sender, EventArgs e)
        {
            Debug.Assert(Building.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

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
            Debug.Assert(Building.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            UpdateData();// Это не надо по идее - Building = pb вызывает UpdateData
        }

        internal void UpdateData()
        {
            Debug.Assert(Building.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            btnHeroes.Parent = null;
            btnBuyOrUpgrade.Parent = null;
            btnHireHero.Parent = null;

            lblName.Text = Building.Building.Name;
            lblIncome.ImageIndex = Building.DoIncome() == true ? FormMain.GUI_16_GOLD : -1;
            lblIncome.Text = Building.DoIncome() == true ? "+" + Building.Income().ToString() : "";
            lblIncome.ForeColor = Building.Level > 0 ? Color.Green : Color.Gray;

            // Восстановить
            //btnHeroes.Visible = building.Building.TrainedHero != null;

            // Информация об уровне здания
            if ((Building.Level > 0) && (Building.Building.MaxLevel > 1))
            {
                //lblLevel.Parent = Parent;

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
            //else
             //   lblLevel.Parent = null;

            // Восстановить
            /*
            if (Building.Building.TrainedHero != null)
            {
                btnHireHero.Show();
                btnHireHero.ImageIndex = (Building.Level > 0) && ((Building.Heroes.Count == Building.MaxHeroes()) || (Building.MaxHeroesAtPlayer() == true))  ? -1 : GuiUtils.GetImageIndexWithGray(btnHireHero.ImageList, Building.Building.TrainedHero.ImageIndex, Building.CanTrainHero());
                btnHireHero.Text = (Building.Level == 0) || (Building.CanTrainHero() == true) ? Building.Building.TrainedHero.Cost.ToString() : ""; 
            }
            else
                btnHireHero.Hide();
            */
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
                pbBuilding.Image = GuiUtils.GetImageFromImageList(Program.formMain.ilBuildings, Building.Building.ImageIndex, true);
            }
            else
            {
                lblName.ForeColor = Color.Gray;

                btnBuyOrUpgrade.Text = Building.CostBuyOrUpgrade().ToString();
                btnBuyOrUpgrade.ImageIndex = GuiUtils.GetImageIndexWithGray(btnBuyOrUpgrade.ImageList, FormMain.GUI_BUY, Building.CheckRequirements());
                pbBuilding.Image = GuiUtils.GetImageFromImageList(Program.formMain.ilBuildings, Building.Building.ImageIndex, false);
            }

            // Восстановить
            /*
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
            */

            //Invalidate();
        }
        /*protected override Color ColorBorder()
        {
            return FormMain.Config.ColorBorder(Program.formMain.SelectedPanelBuilding == this);
        }*/
    }
}