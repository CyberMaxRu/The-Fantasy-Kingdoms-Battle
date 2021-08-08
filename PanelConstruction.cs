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
    // Класс панели сооружения Королевства
    internal sealed class PanelConstruction : PanelMapObject
    {
        private readonly VCIconButton btnHeroes;
        private readonly VCIconButton btnBuildOrUpgrade;
        private readonly VCIconButton btnHireHero;
        private readonly VCLabelValue lblIncome;
        private readonly VCLabelValue lblGreatness;

        public PanelConstruction(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            btnHeroes = new VCIconButton(this, imgMapObject.ShiftX, imgMapObject.ShiftY, Program.formMain.ilGui, FormMain.GUI_HOME);
            btnHeroes.ShowHint += BtnHeroes_ShowHint;

            btnHireHero = new VCIconButton(this, imgMapObject.NextLeft(), btnHeroes.NextTop() + FormMain.Config.GridSize + FormMain.Config.GridSizeHalf, Program.formMain.imListObjectsCell, -1);
            btnHireHero.Click += BtnHireHero_Click;
            btnHireHero.ShowHint += BtnHireHero_ShowHint;

            btnBuildOrUpgrade = new VCIconButton(this, imgMapObject.NextLeft(), imgMapObject.NextTop(), Program.formMain.ilGui, FormMain.GUI_BUILD);
            btnBuildOrUpgrade.Click += BtnBuildOrUpgrade_Click;
            btnBuildOrUpgrade.ShowHint += BtnBuildOrUpgrade_ShowHint;

            lblIncome = new VCLabelValue(this, FormMain.Config.GridSize, imgMapObject.NextTop(), Color.Green, true);
            lblIncome.Width = imgMapObject.Width;
            lblIncome.ImageIndex = FormMain.GUI_16_GOLD;
            lblIncome.StringFormat.Alignment = StringAlignment.Near;

            lblGreatness = new VCLabelValue(this, lblIncome.ShiftX, lblIncome.NextTop() - FormMain.Config.GridSizeHalf, Color.Green, true);
            lblGreatness.Width = lblIncome.Width;
            lblGreatness.ImageIndex = FormMain.GUI_16_GREATNESS;
            lblGreatness.StringFormat.Alignment = StringAlignment.Near;
            lblGreatness.Color = FormMain.Config.HintIncome;

            Height = btnBuildOrUpgrade.NextTop();
            Width = btnBuildOrUpgrade.NextLeft();
            lblNameMapObject.Width = Width - (lblNameMapObject.ShiftX * 2);

            btnHeroes.ShiftX = Width - btnHeroes.Width - FormMain.Config.GridSize;
        }

        internal PlayerConstruction Construction { get; private set; }

        private void BtnHeroes_ShowHint(object sender, EventArgs e)
        {
            Construction.PrepareHintForInhabitantCreatures();
        }

        private void BtnHireHero_ShowHint(object sender, EventArgs e)
        {
            Construction.PrepareHintForHireHero();
        }

        private void BtnBuildOrUpgrade_ShowHint(object sender, EventArgs e)
        {
            Construction.PrepareHintForBuildOrUpgrade();
        }

        private void BtnHireHero_Click(object sender, EventArgs e)
        {
            Debug.Assert(Construction.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);
            Debug.Assert(Construction.Level <= Construction.TypeConstruction.MaxLevel);

            SelectThisConstruction();

            if ((Construction.Level > 0) && (Construction.CanTrainHero() == true))
            {
                if (Construction.TypeConstruction.Category == CategoryConstruction.Temple)
                {
                    MessageBox.Show("Найм храмовых героев в этой версии не реализован.", "Отмена", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Construction.HireHero();
                Program.formMain.UpdateListHeroes();
                Program.formMain.SetNeedRedrawFrame();
            }
        }

        private void BtnBuildOrUpgrade_Click(object sender, EventArgs e)
        {
            Debug.Assert(Construction.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            SelectThisConstruction();

            if (Construction.Player.Gold >= Construction.CostBuyOrUpgrade())
            {
                if (Construction.Level == 0)
                    Construction.Build();
                else
                    Construction.Upgrade();                
                
                Program.formMain.SetNeedRedrawFrame();
                Program.formMain.PlayConstructionComplete();
            }
        }

        internal void LinkToPlayer(PlayerConstruction pb)
        {
            Debug.Assert(pb != null);
            Debug.Assert(pb.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            PlayerObject = pb;
            Construction = pb;
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

            int income = Construction.Level > 0 ? Construction.Income() : Construction.IncomeNextLevel();
            if (income > 0)
            {
                lblIncome.Text = $"+{income}";
                lblIncome.Color = FormMain.Config.ColorIncome(Construction.Level > 0);
                lblIncome.Visible = true;
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
                    btnBuildOrUpgrade.Visible = true;
                    btnBuildOrUpgrade.Cost = Construction.CostBuyOrUpgrade().ToString();
                    btnBuildOrUpgrade.ImageIndex = FormMain.GUI_LEVELUP;
                    btnBuildOrUpgrade.ImageIsEnabled = Construction.CheckRequirements();
                }
                else
                {
                    btnBuildOrUpgrade.Visible = false;
                }
            }
            else
            {
                btnBuildOrUpgrade.Visible = Construction.TypeConstruction.Category != CategoryConstruction.Temple;
                if (btnBuildOrUpgrade.Visible)
                {
                    btnBuildOrUpgrade.Cost = Construction.CostBuyOrUpgrade().ToString();
                    btnBuildOrUpgrade.ImageIsEnabled = Construction.CheckRequirements();
                }
            }

            if ((Construction.TypeConstruction.TrainedHero != null) && (Construction.TypeConstruction.Category != CategoryConstruction.Economic))
            {
                //btnHireHero.ImageIndex = (Construction.Level > 0) && ((Construction.Heroes.Count == Construction.MaxHeroes()) || (Construction.MaxHeroesAtPlayer() == true))  ? -1 : GuiUtils.GetImageIndexWithGray(btnHireHero.ImageList, c.TrainedHero.ImageIndex, Construction.CanTrainHero());
                if (Construction.Heroes.Count < Construction.MaxHeroes())
                {
                    btnHireHero.Visible = true;
                    btnHireHero.ImageIndex = ((Construction.Level > 0) && (Construction.MaxHeroesAtPlayer() == true)) ? -1 : Construction.TypeConstruction.TrainedHero.ImageIndex;
                    btnHireHero.ImageIndex = Program.formMain.TreatImageIndex(Construction.TypeConstruction.TrainedHero.ImageIndex, Construction.Player);
                    btnHireHero.ImageIsEnabled = Construction.CanTrainHero();
                    btnHireHero.Cost = (Construction.Level == 0) || (Construction.CanTrainHero() == true) ? Construction.TypeConstruction.TrainedHero.Cost.ToString() : null;
                }
                else
                    btnHireHero.Visible = false;
            }
            else
                btnHireHero.Visible = false;

            imgMapObject.Level = Construction.Level < Construction.TypeConstruction.MaxLevel ? Construction.Level : 0;
            imgMapObject.Quantity = 0;

            if ((Construction.TypeConstruction.TrainedHero != null) && !(Construction.TypeConstruction.TrainedHero is null) && (Construction.Level > 0) && (Construction.Heroes.Count > 0))
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
            Construction.TypeConstruction.PlaySoundSelect();
        }
    }
}