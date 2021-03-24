using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс панели здания
    internal sealed class PanelConstruction : PanelMapObject
    {
        private readonly VCIconButton btnHeroes;
        private readonly VCIconButton btnBuyOrUpgrade;
        private readonly VCIconButton btnHireHero;
        private readonly VCLabelValue lblIncome;
        
        public PanelConstruction(VisualControl parent, int shiftX, int shiftY, TypeConstruction typeConstruction) : base(parent, shiftX, shiftY, typeConstruction)
        {
            TypeConstruction = typeConstruction;

            btnBuyOrUpgrade = new VCIconButton(this, imgMapObject.NextLeft(), imgMapObject.ShiftY, Program.formMain.ilGui, FormMain.GUI_BUILD);
            btnBuyOrUpgrade.Click += BtnBuyOrUprgade_Click;
            btnBuyOrUpgrade.ShowHint += BtnBuyOrUpgrade_ShowHint;

            btnHeroes = new VCIconButton(this, imgMapObject.ShiftX, imgMapObject.NextTop(), Program.formMain.imListObjectsCell, -1);

            if ((TypeConstruction.TrainedHero != null) && !(TypeConstruction is TypeEconomicConstruction))
            {
                btnHireHero = new VCIconButton(this, imgMapObject.NextLeft(), btnBuyOrUpgrade.NextTop(), Program.formMain.imListObjectsCell, -1);
                btnHireHero.Click += BtnHireHero_Click;
                btnHireHero.ShowHint += BtnHireHero_ShowHint;

            }
            else
                btnHeroes.Visible = false;

            if (TypeConstruction is TypeEconomicConstruction)
            {
                lblIncome = new VCLabelValue(this, imgMapObject.NextLeft() - FormMain.Config.GridSize - FormMain.Config.GridSize, imgMapObject.NextTop(), Color.Green);
                lblIncome.ImageIndex = FormMain.GUI_16_INCOME;
                lblIncome.Width = btnBuyOrUpgrade.Width - FormMain.Config.GridSize;
                lblIncome.StringFormat.Alignment = StringAlignment.Near;
            }

            Height = btnHeroes.NextTop();
            Width = btnBuyOrUpgrade.NextLeft();
            lblNameMapObject.Width = Width - (lblNameMapObject.ShiftX * 2);
        }

        internal TypeConstruction TypeConstruction { get; }
        internal PlayerBuilding Building { get => PlayerObject as PlayerBuilding; }


        private void BtnHireHero_ShowHint(object sender, EventArgs e)
        {
            ShowHintBtnHireHero();
        }

        private void BtnBuyOrUpgrade_ShowHint(object sender, EventArgs e)
        {
            ShowHintBtnBuyOrUpgrade();
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
                Program.formMain.UpdateListHeroes();
                Program.formMain.SetNeedRedrawFrame();
            }
        }

        private void BtnBuyOrUprgade_Click(object sender, EventArgs e)
        {
            Debug.Assert(Building.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            SelectThisBuilding();

            if (Building.Player.Gold >= Building.CostBuyOrUpgrade())
            {
                if (Building.BuyOrUpgrade())
                {
                    btnBuyOrUpgrade.DoShowHint();
                    Program.formMain.SetNeedRedrawFrame();
                    Program.formMain.PlayConstructionComplete();
                }
            }
        }

        internal void LinkToPlayer(PlayerBuilding pb)
        {
            Debug.Assert(pb != null);
            Debug.Assert(pb.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);
            Debug.Assert(pb.Building == TypeConstruction);

            PlayerObject = pb;
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Building.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            imgMapObject.ImageIndex = Building.Building.ImageIndex;
            imgMapObject.ImageIsEnabled = Building.Level > 0;

            lblNameMapObject.Text = Building.Building.Name;
            lblNameMapObject.Color = FormMain.Config.ColorMapObjectCaption(Building.Level > 0);

            if (lblIncome != null)
            {
                lblIncome.Text = "+" + (Building.Level > 0 ? Building.Income() : Building.IncomeNextLevel()).ToString();
                lblIncome.Color = Building.Level > 0 ? FormMain.Config.HintIncome : Color.Gray;
            }

            if (Building.Level > 0)
            {

                if (Building.CanLevelUp())
                {
                    btnBuyOrUpgrade.Cost = Building.CostBuyOrUpgrade();
                    btnBuyOrUpgrade.ImageIndex = FormMain.GUI_LEVELUP;
                    btnBuyOrUpgrade.ImageIsEnabled = Building.CheckRequirements();
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
                btnBuyOrUpgrade.ImageIsEnabled = Building.CheckRequirements();
            }

            if (btnHireHero != null)
            {
                //btnHireHero.ImageIndex = (Building.Level > 0) && ((Building.Heroes.Count == Building.MaxHeroes()) || (Building.MaxHeroesAtPlayer() == true))  ? -1 : GuiUtils.GetImageIndexWithGray(btnHireHero.ImageList, c.TrainedHero.ImageIndex, Building.CanTrainHero());
                btnHireHero.ImageIndex = (Building.Level > 0) && ((Building.Heroes.Count == Building.MaxHeroes()) || (Building.MaxHeroesAtPlayer() == true)) ? -1 : TypeConstruction.TrainedHero.ImageIndex;
                btnHireHero.ImageIndex = Program.formMain.TreatImageIndex(Building.Building.TrainedHero.ImageIndex, Building.Player);
                btnHireHero.ImageIsEnabled = Building.CanTrainHero();
                btnHireHero.Cost = (Building.Level == 0) || (Building.CanTrainHero() == true) ? TypeConstruction.TrainedHero.Cost : 0;
            }

            imgMapObject.Level = Building.Level;

            if ((Building.Building.TrainedHero != null) && (Building.Level > 0) && (Building.Heroes.Count > 0))
            {
                btnHeroes.Cost = Building.Heroes.Count;
                btnHeroes.ImageIndex = Program.formMain.TreatImageIndex(Building.Building.TrainedHero.ImageIndex, Building.Player);
                //btnHeroes.ImageIndex = GuiUtils.GetImageIndexWithGray(btnHeroes.ImageList, Building.Building.TrainedHero.ImageIndex, true);
            }
            else
            {
                btnHeroes.Cost = 0;
                btnHeroes.ImageIndex = -1;
            }

            base.Draw(g);
        }

        protected override void PlaySelect()
        {
            TypeConstruction.PlaySoundSelect();
        }
    }
}