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

        public VCWindowCaption(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, 12)
        {
            labelCaption = new VCLabel(this, WidthCap, 3, Program.formMain.FontMedCaptionC, Color, Program.formMain.FontMedCaptionC.MaxHeightSymbol, "");
            labelCaption.StringFormat.Alignment = StringAlignment.Center;
            labelCaption.StringFormat.LineAlignment = StringAlignment.Near;
            labelCaption.ManualDraw = true;
        }

        internal string Caption { get; set; }
        internal Color Color { get; set; } = Color.MediumAquamarine;
        protected override Bitmap GetBitmap() => Program.formMain.bmpBandWindowCaption;


        internal override void ArrangeControls()
        {
            labelCaption.Width = Width - WidthCap - WidthCap;

            base.ArrangeControls();
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            labelCaption.Text = Caption;
            labelCaption.Color = Color;
            labelCaption.Draw(g);
        }
    }
}
