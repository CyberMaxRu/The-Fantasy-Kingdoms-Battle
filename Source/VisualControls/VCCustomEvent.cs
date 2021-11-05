using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCCustomEvent : VisualControl
    {
        protected readonly VCCellSimple cell;
        protected readonly VCLabel lblCaption;
        protected readonly VCLabel lblText;
        private static Bitmap bmpBackground;

        public VCCustomEvent(Entity entity) : base()
        {
            Debug.Assert(entity != null);

            Entity = entity;

            cell = new VCCellSimple(this, 0, 3);
            cell.ImageIndex = Entity.GetImageIndex();
            cell.HighlightUnderMouse = true;

            lblCaption = new VCLabel(this, cell.NextLeft(), 4, Program.formMain.fontMedCaptionC, Color.Gray, 16, "");
            lblCaption.ClickOnParent = true;

            lblText = new VCLabel(this, lblCaption.ShiftX, 27, Program.formMain.fontMedCaptionC, Color.Gray, 16, "");
            lblText.ClickOnParent = true;
            Height = 54;
        }

        internal override void DrawBackground(Graphics g)
        {
            if (bmpBackground is null)
                bmpBackground = Program.formMain.LoadBitmap("BackgroundEvent.png");

            base.DrawBackground(g);

            g.DrawImageUnscaled(bmpBackground, Left + 52, Top);
        }
    }
}
