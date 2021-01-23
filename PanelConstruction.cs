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
    internal sealed class PanelConstruction : VisualControl
    {
        private readonly VCImage imageConstruction;
        private readonly VCButton btnHeroes;
        private readonly VCButton btnBuyOrUpgrade;
        private readonly VCButton btnHireHero;
        private readonly VCLabel lblName;
        private readonly VCImage imgGold;
        private readonly VCLabel lblIncome;

        public PanelConstruction(VisualControl parent, int shiftX, int shiftY, TypeConstruction typeConstruction) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;

            TypeConstruction = typeConstruction;

            lblName = new VCLabel(this, FormMain.Config.GridSize, FormMain.Config.GridSize, FormMain.Config.FontBuildingCaption, Color.Transparent, FormMain.Config.GridSize * 2, "");
            lblName.StringFormat.Alignment = StringAlignment.Near;
            lblName.Text = typeConstruction.Name;

            imageConstruction = new VCImage(this, FormMain.Config.GridSize, lblName.NextTop(), Program.formMain.ilBuildings, -1);
            imageConstruction.ShowBorder = false;
            imageConstruction.Click += ImageConstruction_Click;
            imageConstruction.ShowHint += ImageConstruction_ShowHint;

            btnBuyOrUpgrade = new VCButton(this, imageConstruction.NextLeft(), imageConstruction.ShiftY, Program.formMain.ilGui, -1);
            btnBuyOrUpgrade.Click += BtnBuyOrUprgade_Click;
            btnBuyOrUpgrade.ShowHint += BtnBuyOrUpgrade_ShowHint;

            btnHeroes = new VCButton(this, imageConstruction.ShiftX, imageConstruction.NextTop(), Program.formMain.ilGuiHeroes, -1);

            if (TypeConstruction.TrainedHero != null)
            {
                btnHireHero = new VCButton(this, imageConstruction.NextLeft(), btnBuyOrUpgrade.NextTop(), Program.formMain.ilGuiHeroes, -1);
                btnHireHero.Click += BtnHireHero_Click;
                btnHireHero.ShowHint += BtnHireHero_ShowHint;

            }
            else
                btnHeroes.Visible = false;

            if (TypeConstruction is TypeEconomicConstruction)
            {
                imgGold = new VCImage(this, imageConstruction.NextLeft(), imageConstruction.NextTop(), Program.formMain.ilGui16, FormMain.GUI_16_GOLD);

                lblIncome = new VCLabel(this, imgGold.ShiftX, imgGold.ShiftY, FormMain.Config.FontToolbar, Color.Green, 16, "");
                lblIncome.Width = btnBuyOrUpgrade.Width;
                lblIncome.StringFormat.Alignment = StringAlignment.Far;
            }

            Height = btnHeroes.NextTop();
            Width = btnBuyOrUpgrade.NextLeft();
            lblName.Width = Width - (lblName.ShiftX * 2);
        }

        internal TypeConstruction TypeConstruction { get; }
        internal PlayerBuilding Building { get; private set; }


        private void BtnHireHero_ShowHint(object sender, EventArgs e)
        {
            ShowHintBtnHireHero();
        }

        private void BtnBuyOrUpgrade_ShowHint(object sender, EventArgs e)
        {
            ShowHintBtnBuyOrUpgrade();
        }

        private void ImageConstruction_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddStep1Header(Building.Building.Name, Building.Level > 0 ? "Уровень " + Building.Level.ToString() : "", Building.Building.Description + ((Building.Level > 0) && (Building.Building.TrainedHero != null) ? Environment.NewLine + Environment.NewLine + "Героев: " + Building.Heroes.Count.ToString() + "/" + Building.MaxHeroes().ToString() : ""));
            Program.formMain.formHint.AddStep2Income(Building.Income());
        }

        private void ImageConstruction_Click(object sender, EventArgs e)
        {
            SelectThisBuilding();
        }

        private void SelectThisBuilding()
        {
            Program.formMain.SelectBuilding(this);
        }

        private void ShowHintBtnHireHero()
        {
            Program.formMain.formHint.AddStep1Header(TypeConstruction.TrainedHero.Name, "", TypeConstruction.TrainedHero.Description);
            if ((TypeConstruction.TrainedHero != null) && (TypeConstruction.TrainedHero.Cost > 0))
                Program.formMain.formHint.AddStep3Requirement(Building.GetTextRequirementsHire());
            Program.formMain.formHint.AddStep4Gold(TypeConstruction.TrainedHero.Cost, Building.Player.Gold >= TypeConstruction.TrainedHero.Cost);
        }

        private void ShowHintBtnBuyOrUpgrade()
        {
            //Debug.Assert(Building.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            if (Building.Level < Building.Building.MaxLevel)
            {
                Program.formMain.formHint.AddStep1Header(Building.Building.Name, Building.Level == 0 ? "Уровень 1" : (Building.CanLevelUp() == true) ? "Улучшить строение" : "", Building.Level == 0 ? Building.Building.Description : "");
                Program.formMain.formHint.AddStep2Income(Building.IncomeNextLevel());
                Program.formMain.formHint.AddStep3Requirement(Building.GetTextRequirements());
                Program.formMain.formHint.AddStep4Gold(Building.CostBuyOrUpgrade(), Building.Player.Gold >= Building.CostBuyOrUpgrade());
                Program.formMain.formHint.AddStep5Builders(Building.Player.EnoughPointConstruction(Building));
            }
        }

        private void BtnHireHero_Click(object sender, EventArgs e)
        {
            Debug.Assert(Building.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);
            Debug.Assert(Building.Level <= Building.Building.MaxLevel);

            SelectThisBuilding();

            if ((Building.Level > 0) && (Building.CanTrainHero() == true))
            {
                if (Building.Building is TypeTemple)
                {
                    MessageBox.Show("Найм храмовых героев в этой версии не реализован.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Building.HireHero();
                btnHireHero.DoShowHint();
                Program.formMain.ShowFrame();
            }
        }

        private void BtnBuyOrUprgade_Click(object sender, EventArgs e)
        {
            Debug.Assert(Building.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            SelectThisBuilding();

            if (Building.Player.Gold >= Building.CostBuyOrUpgrade())
            {
                Building.BuyOrUpgrade();
                btnBuyOrUpgrade.DoShowHint();
                Program.formMain.ShowFrame();
            }
        }

        internal void LinkToPlayer(PlayerBuilding pb)
        {
            Debug.Assert(pb != null);
            Debug.Assert(pb.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);
            Debug.Assert(pb.Building == TypeConstruction);

            Building = pb;
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Building.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            BorderColor = FormMain.Config.ColorBorder(Program.formMain.SelectedPanelBuilding == this);
            imageConstruction.ImageIndex = Building.Building.ImageIndex;
            imageConstruction.NormalImage = Building.Level > 0;

            lblName.Color = FormMain.Config.ColorMapObjectCaption(Building.Level > 0);

            if (lblIncome != null)
            {
                lblIncome.Text = "+" + (Building.Level > 0 ? Building.Income() : Building.IncomeNextLevel()).ToString();
                lblIncome.Color = Building.Level > 0 ? Color.Green : Color.Gray;
            }

            if (Building.Level > 0)
            {

                if (Building.CanLevelUp())
                {
                    btnBuyOrUpgrade.Cost = Building.CostBuyOrUpgrade();
                    btnBuyOrUpgrade.ImageIndex = GuiUtils.GetImageIndexWithGray(btnBuyOrUpgrade.ImageList, FormMain.GUI_LEVELUP, Building.CheckRequirements());
                }
                else
                {
                    btnBuyOrUpgrade.Cost = 0;
                    btnBuyOrUpgrade.ImageIndex = -1;
                }

                //if (btnLevelUp.Visible == true)
                //btnLevelUp.Image = Building.CheckRequirements() == true ? imageListGui.Images[FormMain.GUI_LEVELUP] : imageListGui.Images[FormMain.GUI_LEVELUP + imageListGui.Images.Count / 2];
            }
            else
            {
                btnBuyOrUpgrade.Cost = Building.CostBuyOrUpgrade();
                btnBuyOrUpgrade.ImageIndex = GuiUtils.GetImageIndexWithGray(btnBuyOrUpgrade.ImageList, FormMain.GUI_BUY, Building.CheckRequirements());
            }

            if (btnHireHero != null)
            {
                TypeConstructionWithHero c = (TypeConstructionWithHero)Building.Building;
                btnHireHero.ImageIndex = (Building.Level > 0) && ((Building.Heroes.Count == Building.MaxHeroes()) || (Building.MaxHeroesAtPlayer() == true))  ? -1 : GuiUtils.GetImageIndexWithGray(btnHireHero.ImageList, c.TrainedHero.ImageIndex, Building.CanTrainHero());
                btnHireHero.Cost = (Building.Level == 0) || (Building.CanTrainHero() == true) ? c.TrainedHero.Cost : 0;
            }

            imageConstruction.Level = Building.Level;

            if ((Building.Building.TrainedHero != null) && (Building.Level > 0) && (Building.Heroes.Count > 0))
            {
                btnHeroes.Cost = Building.Heroes.Count;
                btnHeroes.ImageIndex = GuiUtils.GetImageIndexWithGray(btnHeroes.ImageList, Building.Building.TrainedHero.ImageIndex, true);
            }
            else
            {
                btnHeroes.Cost = 0;
                btnHeroes.ImageIndex = -1;
            }

            base.Draw(g);
        }
    }
}