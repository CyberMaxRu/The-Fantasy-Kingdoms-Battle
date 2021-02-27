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
    // Класс панели логова
    internal sealed class PanelLair : VisualControl
    {
        private readonly VCLabel lblName;
        private readonly VCImage imgLair;
        private readonly VCButton btnAction;
        private readonly VCButton btnCancel;
        private readonly VCButton btnInhabitants;

        public PanelLair(VisualControl parent, int shiftX, int shiftY, TypeLair typeLair) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;
            TypeLair = typeLair;

            lblName = new VCLabel(this, FormMain.Config.GridSize, FormMain.Config.GridSize, FormMain.Config.FontBuildingCaption, Color.Transparent, FormMain.Config.GridSize * 2, "");
            lblName.StringFormat.Alignment = StringAlignment.Near;

            imgLair = new VCImage(this, FormMain.Config.GridSize, lblName.NextTop(), Program.formMain.imListObjectsBig, typeLair.ImageIndex);
            imgLair.ShowBorder = false;
            imgLair.HighlightUnderMouse = true;
            imgLair.Click += ImgLair_Click;
            imgLair.ShowHint += ImgLair_ShowHint;

            btnAction = new VCButton(this, imgLair.NextLeft(), imgLair.ShiftY, Program.formMain.ilGui, FormMain.GUI_BATTLE);
            btnAction.Click += BtnAction_Click;
            btnAction.ShowHint += BtnAction_ShowHint;

            btnCancel = new VCButton(this, btnAction.ShiftX, btnAction.NextTop(), Program.formMain.ilGui, FormMain.GUI_FLAG_CANCEL);
            btnCancel.Click += BtnCancel_Click;
            btnCancel.ShowHint += BtnCancel_ShowHint;

            btnInhabitants = new VCButton(this, imgLair.ShiftX, imgLair.NextTop(), Program.formMain.ilGui, FormMain.GUI_HOME);
            btnInhabitants.Click += BtnInhabitants_Click;

            Height = btnInhabitants.NextTop();
            Width = btnAction.NextLeft();

            lblName.Width = Width - (lblName.ShiftX * 2);            
        }

        private void BtnCancel_ShowHint(object sender, EventArgs e)
        {
            if (Lair.Hidden)
            {
                string textReturn = Lair.Cashback() > 0 ? "Возврат денег" : "";
                Program.formMain.formHint.AddStep1Header("Отмена флага разведки", "", textReturn);
                if (Lair.Cashback() > 0)
                    Program.formMain.formHint.AddStep2Income(Lair.Cashback());
            }
            else
                Program.formMain.formHint.AddStep1Header("Отмена флага атаки", "", "");
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Lair.CancelFlag();
        }

        private void BtnAction_ShowHint(object sender, EventArgs e)
        {
            if (Lair.Hidden)
            {
                if (Lair.PriorityFlag == 0)
                    Program.formMain.formHint.AddStep1Header("Разведка", "", "Установить флаг разведки для отправки героев к логову");
                else
                    Program.formMain.formHint.AddStep1Header("Разведка", "Приоритет " + Lair.PriorityFlag.ToString(), "Повысить приоритет разведки логова");

                Program.formMain.formHint.AddStep4Gold(Lair.RequiredGold(), Lair.Player.Gold >= Lair.RequiredGold());
            }
            else
            {

            }
        }

        private void BtnInhabitants_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectLair(this);
            Program.formMain.panelLairInfo.SelectPageInhabitants();
        }

        internal TypeLair TypeLair { get; set; }
        internal PlayerLair Lair { get; private set; }

        internal void LinkToPlayer(PlayerLair pl)
        {
            Debug.Assert(pl != null);
            Debug.Assert(pl.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);
            Debug.Assert(pl.TypeLair == TypeLair);

            Lair = pl;
        }

        private void ImgLair_ShowHint(object sender, EventArgs e)
        {
            Lair.PrepareHint();
        }

        private void ImgLair_Click(object sender, EventArgs e)
        {
            SelectThisBuilding();
        }

        private void BtnAction_Click(object sender, EventArgs e)
        {
            Lair.IncPriority();
        }

        private void SelectThisBuilding()
        {
            Program.formMain.SelectLair(this);
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Lair.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            SetColorBorder(FormMain.Config.ColorBorder(Program.formMain.SelectedPanelLair == this));
            imgLair.ImageIndex = Lair.ImageIndexLair();
            lblName.Text = Lair.NameLair();
            lblName.Color = Lair.Player.TargetLair == Lair ? Color.OrangeRed : Color.Green;
            btnAction.ImageIsEnabled = Lair.CheckRequirements();
            btnCancel.Visible = Lair.PriorityFlag > 0;

            if (Lair.Hidden)
            {
                btnAction.ImageIndex = FormMain.GUI_FLAG_SCOUT;
                btnAction.Cost = Lair.CostScout();
                btnInhabitants.Visible = false;
            }
            else
            {
                btnAction.ImageIndex = FormMain.GUI_FLAG_ATTACK;
                btnAction.Cost = Lair.CostAttack();
                btnInhabitants.Visible = true;
                btnInhabitants.Cost = Lair.CombatHeroes.Count;
            }

            base.Draw(g);
        }
    }
}