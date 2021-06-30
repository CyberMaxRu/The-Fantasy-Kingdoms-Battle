using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - заголовок окна

    internal sealed class VCWindowCaption : VCBitmapBand
    {
        private readonly VCLabel labelCaption;

        public VCWindowCaption(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            labelCaption = new VCLabel(this, WidthCap(), 3, Program.formMain.fontMedCaptionC, Color.MediumAquamarine, Program.formMain.fontMedCaptionC.MaxHeightSymbol, "");
            labelCaption.StringFormat.Alignment = StringAlignment.Center;
            labelCaption.StringFormat.LineAlignment = StringAlignment.Near;
            labelCaption.ManualDraw = true;
        }

        internal string Caption { get; set; }
        protected override int WidthCap() => 12;
        protected override Bitmap GetBitmap() => Program.formMain.bmpBandWindowCaption;


        internal override void ArrangeControls()
        {
            labelCaption.Width = Width - WidthCap() - WidthCap();

            base.ArrangeControls();
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            labelCaption.Text = Caption;
            labelCaption.Draw(g);
        }
    }
}
