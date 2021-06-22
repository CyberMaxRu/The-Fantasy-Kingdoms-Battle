using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Панель объекта карты
    internal abstract class PanelMapObject : VisualControl
    {
        private Bitmap bmpBackground;
        protected readonly VCLabel lblNameMapObject;
        protected readonly VCImageBig imgMapObject;

        public PanelMapObject(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;

            lblNameMapObject = new VCLabel(this, FormMain.Config.GridSize, FormMain.Config.GridSize - 3, Program.formMain.fontMedCaptionC, Color.Transparent, FormMain.Config.GridSize * 3, "");
            lblNameMapObject.StringFormat.Alignment = StringAlignment.Center;
            lblNameMapObject.ShowBorder = true;
            lblNameMapObject.Click += ImgLair_Click;

            imgMapObject = new VCImageBig(this, lblNameMapObject.NextTop());
            imgMapObject.HighlightUnderMouse = true;
            imgMapObject.Click += ImgLair_Click;
            imgMapObject.ShowHint += ImgLair_ShowHint;

            Click += ImgLair_Click;
        }
        protected abstract void PlaySelect();

        protected override void ValidateRectangle()
        {
            base.ValidateRectangle();

            lblNameMapObject.Width = Width - (lblNameMapObject.ShiftX * 2);
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            if ((bmpBackground == null) || (bmpBackground.Width != Width) || (bmpBackground.Height != Height))
            {
                bmpBackground?.Dispose();
                bmpBackground = GuiUtils.MakeBackground(new Size(Width, Height));
            }

            g.DrawImageUnscaled(bmpBackground, Left, Top);
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            Debug.Assert(lblNameMapObject.Text.Length > 0);
        }

        private void ImgLair_ShowHint(object sender, EventArgs e)
        {
            PlayerObject.PrepareHint();
        }

        private void ImgLair_Click(object sender, EventArgs e)
        {
            if (!Selected())
                PlaySelect();
            SelectThisBuilding();
        }

        protected void SelectThisBuilding()
        {
            Debug.Assert(PlayerObject != null);
            Program.formMain.SelectPlayerObject(PlayerObject);
        }

        protected override bool Selected()
        {
            return (PlayerObject != null) && Program.formMain.PlayerObjectIsSelected(PlayerObject);
        }
    }
}
