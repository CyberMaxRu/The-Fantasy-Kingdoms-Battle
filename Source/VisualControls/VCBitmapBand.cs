using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - растягиваемая картинка
    // Состоит из левой части, середины (повторяемой) и правой части

    internal abstract class VCBitmapBand : VisualControl
    {
        private Bitmap bmp;

        public VCBitmapBand(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            Height = GetBitmap().Height;
        }

        internal override void Draw(Graphics g)
        {
            Bitmap bmpBand = GetBitmap();
            Debug.Assert(Width >= bmpBand.Width);

            if ((bmp == null) || (bmp.Width != Width))
            {
                bmp?.Dispose();
                int widthCap = WidthCap();
                int widthBody = bmpBand.Width - widthCap - widthCap;
                Debug.Assert(widthBody > 0);

                bmp = new Bitmap(Width, Program.formMain.bmpSeparator.Height);
                Graphics gb = Graphics.FromImage(bmp);

                gb.DrawImage(bmpBand, 0, 0, new Rectangle(0, 0, widthCap, widthCap), GraphicsUnit.Pixel);
                gb.DrawImage(bmpBand, bmp.Width - widthCap, 0, new Rectangle(widthBody + widthBody, 0, widthCap, widthCap), GraphicsUnit.Pixel);

                Rectangle rectBand = new Rectangle(widthCap, 0, widthBody, widthCap);
                for (int i = widthCap; i < bmp.Width - widthCap; i++)
                {
                    gb.DrawImage(bmpBand, i, 0, rectBand, GraphicsUnit.Pixel);
                }

                gb.Dispose();
            }

            base.Draw(g);

            g.DrawImageUnscaled(bmp, Left, Top);
        }

        protected abstract int WidthCap();
        protected abstract Bitmap GetBitmap();
    }
}
