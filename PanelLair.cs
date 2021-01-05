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
    internal sealed class PanelLair : BasePanel
    {
        private PlayerLair lair;
        private readonly Label lblName;
        private readonly PictureBox pbLair;
        private readonly Button btnSetAsTarget;

        public PanelLair() : base()
        {
            lblName = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Height = FormMain.Config.GridSize * 2,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontBuildingCaption
            };

            pbLair = new PictureBox()
            {
                Parent = this,
                Width = Program.formMain.ilLairs.ImageSize.Width,
                Height = Program.formMain.ilLairs.ImageSize.Height,
                Left = FormMain.Config.GridSize,
                Top = GuiUtils.NextTop(lblName),
                BackColor = Color.Transparent
            };
            pbLair.MouseEnter += PbLair_MouseEnter;
            pbLair.MouseLeave += PbLair_MouseLeave;
            pbLair.MouseClick += PbLair_MouseClick;

            btnSetAsTarget = new Button()
            {
                Parent = this,
                Left = GuiUtils.NextLeft(pbLair),
                Top = pbLair.Top,
                Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGui),
                ImageList = Program.formMain.ilGui,
                ImageIndex = FormMain.GUI_BATTLE,
                TextAlign = ContentAlignment.BottomCenter,
                FlatStyle = FlatStyle.Flat,
                Font = FormMain.Config.FontCost,
                BackgroundImage = Program.formMain.bmpBackgroundButton,
                ForeColor = Color.White
            };
            btnSetAsTarget.Click += BtnSetAsTarget_Click;

            Height = GuiUtils.NextTop(pbLair);// lblIncome. Top + lblIncome.Height + (Config.GRID_SIZE * 2);
            Width = btnSetAsTarget.Left + btnSetAsTarget.Width + FormMain.Config.GridSize;// btnBuyOrUpgrade.Left + btnBuyOrUpgrade.Width + FormMain.Config.GridSize;

            lblName.Width = Width - (FormMain.Config.GridSize * 2) - 2;
            //lblLevel.Left = Width - FormMain.Config.GridSize - lblLevel.Width;

            MouseClick += PanelLair_MouseClick;
        }

        private void BtnSetAsTarget_Click(object sender, EventArgs e)
        {
            Program.formMain.UpdateTarget(lair);
        }

        private void PbLair_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SelectThisBuilding();
        }

        private void PbLair_MouseLeave(object sender, EventArgs e)
        {
            Program.formMain.formHint.HideHint();
        }

        private void PbLair_MouseEnter(object sender, EventArgs e)
        {
            Program.formMain.formHint.Clear();
            Program.formMain.formHint.AddStep1Header(Lair.Lair.Name, "", Lair.Lair.Description);
            Program.formMain.formHint.ShowHint(this);
        }

        private void PanelLair_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SelectThisBuilding();
        }
        private void SelectThisBuilding()
        {
            Program.formMain.SelectLair(this);
        }

        internal PlayerLair Lair { get { return lair; } set { lair = value; UpdateData(); } }

        internal void ShowData(PlayerLair pl)
        {
            Debug.Assert(pl != null);

            Lair = pl;
        }

        private void UpdateData()
        {
            Debug.Assert(Lair.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            lblName.Text = lair.Lair.Name;
            lblName.ForeColor = lair.Player.TargetLair == lair ? Color.OrangeRed : Color.Green;
            btnSetAsTarget.FlatAppearance.BorderColor = lair.Player.TargetLair == lair ? Color.OrangeRed : Color.Black;
            pbLair.Image = GuiUtils.GetImageFromImageList(Program.formMain.ilLairs, lair.Lair.ImageIndex, true);
        }

        protected override Color ColorBorder()
        {
            return FormMain.Config.ColorBorder(Program.formMain.SelectedPanelLair == this);
        }
    }
}