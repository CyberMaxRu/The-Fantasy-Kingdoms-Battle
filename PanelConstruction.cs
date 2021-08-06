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
        private readonly VCLabelValue lblGreatness;

        public PanelConstruction(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            btnHeroes = new VCIconButton(this, imgMapObject.ShiftX, imgMapObject.ShiftY, Program.formMain.ilGui, FormMain.GUI_HOME);

            btnHireHero = new VCIconButton(this, imgMapObject.NextLeft(), btnHeroes.NextTop() + FormMain.Config.GridSize + FormMain.Config.GridSizeHalf, Program.formMain.imListObjectsCell, -1);
            btnHireHero.Click += BtnHireHero_Click;
            btnHireHero.ShowHint += BtnHireHero_ShowHint;

            btnBuyOrUpgrade = new VCIconButton(this, imgMapObject.NextLeft(), imgMapObject.NextTop(), Program.formMain.ilGui, FormMain.GUI_BUILD);
            btnBuyOrUpgrade.Click += BtnBuyOrUprgade_Click;
            btnBuyOrUpgrade.ShowHint += BtnBuyOrUpgrade_ShowHint;


            //if ((TypeConstruction.TrainedHero != null) && !(TypeConstruction is TypeEconomicConstruction))
            //{
            //}
            //else
            //    btnHeroes.Visible = false;

            //if (TypeConstruction is TypeEconomicConstruction)
            //{

            lblIncome = new VCLabelValue(this, FormMain.Config.GridSize, imgMapObject.NextTop(), Color.Green, true);
            lblIncome.Width = imgMapObject.Width;
            lblIncome.ImageIndex = FormMain.GUI_16_GOLD;
            lblIncome.StringFormat.Alignment = StringAlignment.Near;

            lblGreatness = new VCLabelValue(this, lblIncome.ShiftX, lblIncome.NextTop() - FormMain.Config.GridSizeHalf, Color.Green, true);
            lblGreatness.Width = lblIncome.Width;
            lblGreatness.ImageIndex = FormMain.GUI_16_GREATNESS;
            lblGreatness.StringFormat.Alignment = StringAlignment.Near;
            lblGreatness.Color = FormMain.Config.HintIncome;

            //}

            Height = btnBuyOrUpgrade.NextTop();
            Width = btnBuyOrUpgrade.NextLeft();
            lblNameMapObject.Width = Width - (lblNameMapObject.ShiftX * 2);

            btnHeroes.ShiftX = Width - btnHeroes.Width - FormMain.Config.GridSize;
        }

        internal PlayerConstruction Construction { get => PlayerObject as PlayerConstruction; }
        internal TypeConstruction TypeConstruction { get => Construction.TypeConstruction; }


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
                Program.formMain.formHint.AddStep3Requirement(Construction.GetTextRequirementsHire());
            Program.formMain.formHint.AddStep4Gold(TypeConstruction.TrainedHero.Cost, Construction.Player.Gold >= TypeConstruction.TrainedHero.Cost);
        }

        private void ShowHintBtnBuyOrUpgrade()
        {
            //Debug.Assert(Construction.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            if (Construction.Level < Construction.TypeConstruction.MaxLevel)
            {
                if (Construction.TypeConstruction.LevelAsQuantity)
                    Program.formMain.formHint.AddStep1Header(Construction.TypeConstruction.Name, "Построить сооружение", Construction.Level == 0 ? Construction.TypeConstruction.Description : "");
                else
                    Program.formMain.formHint.AddStep1Header(Construction.TypeConstruction.Name, Construction.Level == 0 ? "Уровень 1" : (Construction.CanLevelUp() == true) ? "Улучшить строение" : "", Construction.Level == 0 ? Construction.TypeConstruction.Description : "");

                Program.formMain.formHint.AddStep2Income(Construction.IncomeNextLevel());
                Program.formMain.formHint.AddStep3Greatness(Construction.GreatnessAddNextLevel(), Construction.GreatnessPerDayNextLevel());
                Program.formMain.formHint.AddStep35PlusBuilders(Construction.BuildersPerDayNextLevel());
                Program.formMain.formHint.AddStep3Requirement(Construction.GetTextRequirements());
                Program.formMain.formHint.AddStep4Gold(Construction.CostBuyOrUpgrade(), Construction.Player.Gold >= Construction.CostBuyOrUpgrade());
                Program.formMain.formHint.AddStep5Builders(Construction.TypeConstruction.Levels[Construction.Level + 1].Builders, Construction.Player.FreeBuilders >= Construction.TypeConstruction.Levels[Construction.Level + 1].Builders);
            }
        }

        private void BtnHireHero_Click(object sender, EventArgs e)
        {
            Debug.Assert(Construction.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);
            Debug.Assert(Construction.Level <= Construction.TypeConstruction.MaxLevel);

            SelectThisConstruction();

            if ((Construction.Level > 0) && (Construction.CanTrainHero() == true))
            {
                if (Construction.TypeConstruction is TypeTemple)
                {
                    MessageBox.Show("Найм храмовых героев в этой версии не реализован.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Construction.HireHero();
                Program.formMain.UpdateListHeroes();
                Program.formMain.SetNeedRedrawFrame();
            }
        }

        private void BtnBuyOrUprgade_Click(object sender, EventArgs e)
        {
            Debug.Assert(Construction.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            SelectThisConstruction();

            if (Construction.Player.Gold >= Construction.CostBuyOrUpgrade())
            {
                if (Construction.BuyOrUpgrade())
                {
                    Program.formMain.SetNeedRedrawFrame();
                    Program.formMain.PlayConstructionComplete();
                }
            }
        }

        internal void LinkToPlayer(PlayerConstruction pb)
        {
            Debug.Assert(pb != null);
            Debug.Assert(pb.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            PlayerObject = pb;
        }

        internal override void ArrangeControls()
        {

            base.ArrangeControls();
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Construction.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            imgMapObject.ImageIndex = Construction.TypeConstruction.ImageIndex;
            imgMapObject.NormalImage = Construction.Level > 0;

            lblNameMapObject.Text = Construction.TypeConstruction.Name;
            lblNameMapObject.Color = FormMain.Config.ColorMapObjectCaption(Construction.Level > 0);

            if (Construction.TypeConstruction is TypeEconomicConstruction)
            {
                int income = Construction.Level > 0 ? Construction.Income() : Construction.IncomeNextLevel();
                if (income > 0)
                {
                    lblIncome.Text = $"+{income}";
                    lblIncome.Color = FormMain.Config.ColorIncome(Construction.Level > 0);
                    lblIncome.Visible = true;
                }
                else
                    lblIncome.Visible = false;
            }
            else
                lblIncome.Visible = false;

            int greatness = Construction.Level > 0 ? Construction.GreatnessPerDay() : Construction.GreatnessPerDayNextLevel();
            lblGreatness.Visible = greatness > 0;
            if (lblGreatness.Visible)
            {
                lblGreatness.Text = $"+{greatness}";
                lblGreatness.Color = FormMain.Config.ColorGreatness(Construction.Level > 0);
            }

            if (Construction.Level > 0)
            {
                if (Construction.CanLevelUp())
                {
                    btnBuyOrUpgrade.Visible = true;
                    btnBuyOrUpgrade.Cost = Construction.CostBuyOrUpgrade().ToString();
                    btnBuyOrUpgrade.ImageIndex = Construction.TypeConstruction.LevelAsQuantity ? FormMain.GUI_BUILD : FormMain.GUI_LEVELUP;
                    btnBuyOrUpgrade.ImageIsEnabled = Construction.CheckRequirements();
                }
                else
                {
                    btnBuyOrUpgrade.Visible = false;
                }
            }
            else
            {
                btnBuyOrUpgrade.Visible = true;
                btnBuyOrUpgrade.Cost = Construction.CostBuyOrUpgrade().ToString();
                btnBuyOrUpgrade.ImageIsEnabled = Construction.CheckRequirements();
            }

            if ((TypeConstruction.TrainedHero != null) && !(TypeConstruction is TypeEconomicConstruction))
            {
                //btnHireHero.ImageIndex = (Construction.Level > 0) && ((Construction.Heroes.Count == Construction.MaxHeroes()) || (Construction.MaxHeroesAtPlayer() == true))  ? -1 : GuiUtils.GetImageIndexWithGray(btnHireHero.ImageList, c.TrainedHero.ImageIndex, Construction.CanTrainHero());
                if (Construction.Heroes.Count < Construction.MaxHeroes())
                {
                    btnHireHero.Visible = true;
                    btnHireHero.ImageIndex = ((Construction.Level > 0) && (Construction.MaxHeroesAtPlayer() == true)) ? -1 : TypeConstruction.TrainedHero.ImageIndex;
                    btnHireHero.ImageIndex = Program.formMain.TreatImageIndex(Construction.TypeConstruction.TrainedHero.ImageIndex, Construction.Player);
                    btnHireHero.ImageIsEnabled = Construction.CanTrainHero();
                    btnHireHero.Cost = (Construction.Level == 0) || (Construction.CanTrainHero() == true) ? TypeConstruction.TrainedHero.Cost.ToString() : null;
                }
                else
                    btnHireHero.Visible = false;
            }
            else
                btnHireHero.Visible = false;

            imgMapObject.Level = Construction.TypeConstruction.LevelAsQuantity ? 0 : Construction.Level < Construction.TypeConstruction.MaxLevel ? Construction.Level : 0;
            imgMapObject.Quantity = Construction.TypeConstruction.LevelAsQuantity ? Construction.Level : 0;

            if ((Construction.TypeConstruction.TrainedHero != null) && !(TypeConstruction is TypeEconomicConstruction) && (Construction.Level > 0) && (Construction.Heroes.Count > 0))
            {
                btnHeroes.Cost = Construction.Heroes.Count.ToString() + "/" + Construction.MaxHeroes();
                //btnHeroes.ImageIndex = Program.formMain.TreatImageIndex(Construction.TypeConstruction.TrainedHero.ImageIndex, Construction.Player);
                btnHeroes.Visible = true;
            }
            else
            {
                btnHeroes.Cost = null;
                btnHeroes.Visible = false;
            }

            base.Draw(g);
        }

        protected override void PlaySelect()
        {
            TypeConstruction.PlaySoundSelect();
        }
    }
}