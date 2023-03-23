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
        public VCLabelValue(VisualControl parent, int shiftX, int shiftY, Color foreColor, bool showBackground)
            : base(parent, shiftX, shiftY, Program.formMain.FontParagraph, foreColor, showBackground ? 24 : 16, "", Program.formMain.BmpListGui16)
        {
            StringFormat.Alignment = StringAlignment.Near;
            ShowBackground = showBackground;
            IsActiveControl = true;

            if (!showBackground)
            {
                TopMargin = -4;
            }
            else
            {
                ShiftImage = new Point(4, 4);
                TopMargin = 0;
                LeftMargin = 0;
                RightMargin = 4;
            }
        }

        internal bool ShowBackground { get; }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            if (ShowBackground)
                Program.formMain.bbIcon16.DrawBorder(g, Left, Top, Width, Program.formMain.bbIcon16.Height, Color.Transparent);
        }
    }
}
