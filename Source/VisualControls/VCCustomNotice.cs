using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCCustomNotice : VisualControl
    {
        protected readonly VCCellSimple cell;
        protected readonly VCLabel lblCaption;
        protected readonly VCLabel lblText;
        private static Bitmap bmpBackground;

        public VCCustomNotice() : base()
        {
            cell = new VCCellSimple(this, 0, 3);

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

        internal void SetNotice(int imageIndex, string caption, string text, Color colorText)
        {
            cell.ImageIndex = imageIndex;
            lblCaption.Text = caption;
            lblText.Text = text;
            lblText.Color = colorText;

            lblCaption.Width = lblCaption.Font.WidthText(lblCaption.Text);
            lblText.Width = lblText.Font.WidthText(lblText.Text);
        }
    }
}
