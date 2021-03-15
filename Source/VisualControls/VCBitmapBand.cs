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
            if ((bmp == null) || (bmp.Width != Width))
            {
                AdjustSize();
            }

            base.Draw(g);

            g.DrawImageUnscaled(bmp, Left, Top);
        }

        protected virtual void AdjustSize()
        {
            Bitmap bmpBand = GetBitmap();
            Debug.Assert(Width >= bmpBand.Width);

            bmp?.Dispose();
            int widthCap = WidthCap();
            int widthBody = bmpBand.Width - widthCap - widthCap;
            Debug.Assert(widthBody > 0);

            bmp = new Bitmap(Width, bmpBand.Height);
            Graphics gb = Graphics.FromImage(bmp);

            gb.DrawImage(bmpBand, 0, 0, new Rectangle(0, 0, widthCap, bmpBand.Height), GraphicsUnit.Pixel);
            gb.DrawImage(bmpBand, bmp.Width - widthCap, 0, new Rectangle(widthBody + widthCap, 0, widthCap, bmpBand.Height), GraphicsUnit.Pixel);

            Bitmap bmpBody = new Bitmap(widthBody, bmpBand.Height);
            Graphics gBody = Graphics.FromImage(bmpBody);
            gBody.DrawImage(bmpBand, 0, 0, new Rectangle(widthCap, 0, widthBody, bmpBand.Height), GraphicsUnit.Pixel);
            int widthForBand = bmp.Width - widthCap - widthCap;
            int repeats = widthForBand / widthBody;
            int restBorder = widthForBand - (widthBody * repeats);
            for (int i = 0; i < repeats; i++)
            {
                gb.DrawImageUnscaled(bmpBody, widthCap + (widthBody * i), 0);
            }
            gb.DrawImageUnscaledAndClipped(bmpBody, new Rectangle(widthCap + (widthBody * repeats), 0, restBorder, bmpBody.Height));

            gBody.Dispose();
            bmpBody.Dispose();
            gb.Dispose();
        }

        protected abstract int WidthCap();
        protected abstract Bitmap GetBitmap();
    }
}
