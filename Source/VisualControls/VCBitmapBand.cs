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
        protected Bitmap bmpForDraw;

        public VCBitmapBand(VisualControl parent, int shiftX, int shiftY, int widthCap) : base(parent, shiftX, shiftY)
        {
            WidthCap = widthCap;
            Height = GetBitmap().Height;
        }

        internal bool TruncateLeft { get; set; }// Если размер меньше минимального, отрезается изображение слева
        internal int WidthCap { get; }// Ширина боковушки

        internal override void ArrangeControls()
        {
            if ((bmpForDraw == null) || (bmpForDraw.Width != Width))
            {
                bmpForDraw?.Dispose();
                bmpForDraw = PrepareBand(GetBitmap());
            }

            base.ArrangeControls();
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            if (bmpForDraw != null)
                g.DrawImageUnscaled(bmpForDraw, Left, Top);
        }

        protected Bitmap PrepareBand(Bitmap bmpBand)
        {
            Debug.Assert(Width > 0);

            if (!TruncateLeft)
            {
                Debug.Assert(Width >= bmpBand.Width, $"Width={Width}, bmpBand.Width={bmpBand.Width}");
            }

            int widthCap = WidthCap;
            int widthBody = bmpBand.Width - widthCap - widthCap;
            Debug.Assert(widthBody > 0);
            int offsetX = !TruncateLeft || (Width >= bmpBand.Width) ? 0 : Width - bmpBand.Width + 1;

            Bitmap bmp = new Bitmap(Width, bmpBand.Height);
            Graphics gb = Graphics.FromImage(bmp);

            gb.DrawImage(bmpBand, offsetX, 0, new Rectangle(0, 0, widthCap, bmpBand.Height), GraphicsUnit.Pixel);
            gb.DrawImage(bmpBand, bmp.Width - widthCap, 0, new Rectangle(widthBody + widthCap, 0, widthCap, bmpBand.Height), GraphicsUnit.Pixel);

            if (offsetX == 0)
            {
                Bitmap bmpBody = new Bitmap(widthBody, bmpBand.Height);
                Graphics gBody = Graphics.FromImage(bmpBody);
                gBody.DrawImage(bmpBand, 0, 0, new Rectangle(widthCap, 0, widthBody, bmpBand.Height), GraphicsUnit.Pixel);
                int widthForBand = bmp.Width - widthCap - widthCap;
                int repeats = widthForBand / widthBody;
                int restBorder = widthForBand - (widthBody * repeats);
                for (int i = 0; i < repeats; i++)
                {
                    gb.DrawImageUnscaled(bmpBody, offsetX + widthCap + (widthBody * i), 0);
                }

                if (restBorder > 0)
                    gb.DrawImageUnscaledAndClipped(bmpBody, new Rectangle(offsetX + widthCap + (widthBody * repeats), 0, restBorder, bmpBody.Height));

                gBody.Dispose();
                bmpBody.Dispose();
            }

            gb.Dispose();

            return bmp;
        }

        protected abstract Bitmap GetBitmap();
    }
}
