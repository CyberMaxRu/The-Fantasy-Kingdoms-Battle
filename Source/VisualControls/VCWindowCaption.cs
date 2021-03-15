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
        private readonly VCLabelM2 labelCaption;

        public VCWindowCaption(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            labelCaption = new VCLabelM2(this, WidthCap(), -2, Program.formMain.fontBigCaption, Color.MediumAquamarine, GetBitmap().Height, "");
            labelCaption.StringFormat.Alignment = StringAlignment.Center;
            labelCaption.StringFormat.LineAlignment = StringAlignment.Center;
            labelCaption.Visible = false;
        }

        internal string Caption { get; set; }
        protected override int WidthCap() => 12;
        protected override Bitmap GetBitmap() => Program.formMain.bmpBandWindowCaption;

        protected override void AdjustSize()
        {
            base.AdjustSize();

            labelCaption.Width = Width - WidthCap() - WidthCap();
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            labelCaption.Text = Caption;
            labelCaption.Draw(g);
        }
    }
}
