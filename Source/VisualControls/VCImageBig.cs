using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - картинка большого объекта, с рамкой
    internal sealed class VCImageBig : VCImage
    {
        public VCImageBig(VisualControl parent, int shiftY) : base(parent, FormMain.Config.GridSize, shiftY, Program.formMain.imListObjectsBig, -1)
        {
            HighlightUnderMouse = true;
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            // Рисуем бордюр вокруг иконки
            g.DrawImageUnscaled(Program.formMain.bmpBorderBig, Left - 2, Top - 2);
        }
    }
}
