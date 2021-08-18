using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - картинка большого объекта (128 * 128)
    internal sealed class VCImage128 : VCImage
    {
        public VCImage128(VisualControl parent, int shiftY) : base(parent, FormMain.Config.GridSize, shiftY, Program.formMain.imListObjectsBig, -1)
        {
            ShowBorder = true;
            labelQuantity.ShiftX -= FormMain.Config.GridSizeHalf;
        }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            labelQuantity.ShiftY -= FormMain.Config.GridSizeHalf;
        }

        internal override void PaintBorder(Graphics g)
        {
            g.DrawImageUnscaled(Program.formMain.bmpBorderBig, Left - 2, Top - 2);
        }
    }
}
