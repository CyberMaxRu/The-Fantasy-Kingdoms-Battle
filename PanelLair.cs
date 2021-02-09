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
    // Класс панели логова
    internal sealed class PanelLair : VisualControl
    {
        private readonly VCLabel lblName;
        private readonly VCImage imgLair;
        private readonly VCButton btnSetAsTarget;

        public PanelLair(VisualControl parent, int shiftX, int shiftY, TypeLair typeLair) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;
            TypeLair = typeLair;

            lblName = new VCLabel(this, FormMain.Config.GridSize, FormMain.Config.GridSize, FormMain.Config.FontBuildingCaption, Color.Transparent, FormMain.Config.GridSize * 2, "");
            lblName.StringFormat.Alignment = StringAlignment.Near;
            lblName.Text = typeLair.Name;

            imgLair = new VCImage(this, FormMain.Config.GridSize, lblName.NextTop(), Program.formMain.imListObjectsBig, typeLair.ImageIndex);
            imgLair.ShowBorder = false;
            imgLair.Click += ImgLair_Click;
            imgLair.ShowHint += ImgLair_ShowHint;

            btnSetAsTarget = new VCButton(this, imgLair.NextLeft(), imgLair.ShiftY, Program.formMain.ilGui, FormMain.GUI_BATTLE);
            btnSetAsTarget.Click += BtnSetAsTarget_Click;

            Height = imgLair.NextTop();
            Width = btnSetAsTarget.NextLeft();

            lblName.Width = Width - (lblName.ShiftX * 2);            
        }

        internal TypeLair TypeLair { get; set; }
        internal PlayerLair Lair { get; private set; }

        internal void LinkToPlayer(PlayerLair pl)
        {
            Debug.Assert(pl != null);
            Debug.Assert(pl.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);
            Debug.Assert(pl.Lair == TypeLair);

            Lair = pl;
        }

        private void ImgLair_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddStep1Header(Lair.Lair.Name, "", Lair.Lair.Description);
        }

        private void ImgLair_Click(object sender, EventArgs e)
        {
            SelectThisBuilding();
        }

        private void BtnSetAsTarget_Click(object sender, EventArgs e)
        {
            Program.formMain.UpdateTarget(Lair);
        }

        private void SelectThisBuilding()
        {
            Program.formMain.SelectLair(this);
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Lair.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            BorderColor = FormMain.Config.ColorBorder(Program.formMain.SelectedPanelLair == this);
            lblName.Color = Lair.Player.TargetLair == Lair ? Color.OrangeRed : Color.Green;

            base.Draw(g);
        }
    }
}