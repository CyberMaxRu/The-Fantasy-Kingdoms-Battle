using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - разделитель
    internal sealed class VCSeparator : VisualControl
    {
        private Bitmap bmp;

        public VCSeparator(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            Height = Program.formMain.bmpSeparator.Height;
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Width >= Program.formMain.bmpSeparator.Width);

            if ((bmp == null) || (bmp.Width != Width))
            {
                bmp?.Dispose();

                bmp = new Bitmap(Width, Program.formMain.bmpSeparator.Height);
                Graphics gb = Graphics.FromImage(bmp);

                gb.DrawImage(Program.formMain.bmpSeparator, 0, 0, new Rectangle(0, 0, 10, 10), GraphicsUnit.Pixel);
                gb.DrawImage(Program.formMain.bmpSeparator, bmp.Width - 10, 0, new Rectangle(11, 0, 10, 10), GraphicsUnit.Pixel);

                Rectangle rectBand = new Rectangle(10, 0, 1, 10);
                for (int i = 10; i < bmp.Width - 10; i++)
                {
                    gb.DrawImage(Program.formMain.bmpSeparator, i, 0, rectBand, GraphicsUnit.Pixel);
                }

                gb.Dispose();
            }

            base.Draw(g);

            g.DrawImageUnscaled(bmp, Left, Top);
        }
    }
}
