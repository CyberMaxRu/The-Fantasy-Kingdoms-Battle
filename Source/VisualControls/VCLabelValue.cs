using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - метка с иконкой, для отображения значения
    internal sealed class VCLabelValue : VCLabel
    {
        private Bitmap bmpBackround;

        public VCLabelValue(VisualControl parent, int shiftX, int shiftY, Color foreColor, bool showBackground)
            : base(parent, shiftX, shiftY, Program.formMain.fontMedCaption, foreColor, showBackground ? 24 : 16, "")
        {
            BitmapList = Program.formMain.ilGui16;
            StringFormat.Alignment = StringAlignment.Near;
            ShowBackground = showBackground;

            if (!showBackground)
            {
                TopMargin = -2;
            }
            else
            {
                ShiftImage = new Point(4, 4);
                TopMargin = 2;
                LeftMargin = 0;
                RightMargin = 4;
            }
        }

        internal bool ShowBackground { get; }

        protected override void ValidateRectangle()
        {
            base.ValidateRectangle();

            if ((Width > 0) && (Height > 0) && ((bmpBackround == null) || (bmpBackround.Width != Width) || (bmpBackround.Height != Height)))
            {
                bmpBackround?.Dispose();
                bmpBackround = Program.formMain.bbIcon16.DrawBorder(Width, Program.formMain.bbIcon16.Height);
            }
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            if (ShowBackground)
                g.DrawImageUnscaled(bmpBackround, Left, Top);
        }
    }
}
