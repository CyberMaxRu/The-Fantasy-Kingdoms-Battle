using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCIconAndDigitValue : VCLabel
    {
        public VCIconAndDigitValue(VisualControl parent, int shiftX, int shiftY, int width, int imageIndex)
            : base(parent, shiftX, shiftY, Program.formMain.fontMedCaption, Color.White, 24, "", Program.formMain.BmpListGui16)
        {
            StringFormat.Alignment = StringAlignment.Near;
            ShiftImage = new Point(4, 4);
            IsActiveControl = true;
            TopMargin = 2;
            LeftMargin = 0;
            RightMargin = 5;
            Image.ImageIndex = imageIndex;
            StringFormat.Alignment = StringAlignment.Far;
            Width = width;
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            Program.formMain.bbIcon16.DrawBorder(g, Left, Top, Width, Program.formMain.bbIcon16.Height, Color.Transparent);
        }
    }
}
