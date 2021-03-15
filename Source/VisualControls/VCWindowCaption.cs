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
        public VCWindowCaption(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
        }

        protected override int WidthCap() => 12;
        protected override Bitmap GetBitmap() => Program.formMain.bmpBandWindowCaption;
    }

}
