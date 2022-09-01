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
        static private Dictionary<(Bitmap, Color, int), Bitmap> cacheBands = new Dictionary<(Bitmap, Color, int), Bitmap>();

        private Bitmap bmpForDraw;
        private Color color;

        public VCBitmapBand(VisualControl parent, int shiftX, int shiftY, int widthCap) : base(parent, shiftX, shiftY)
        {
            WidthCap = widthCap;
            Height = GetBitmap().Height;
        }

        internal bool TruncateLeft { get; set; }// Если размер меньше минимального, отрезается изображение слева
        internal int WidthCap { get; }// Ширина боковушки
        internal Color Color { get; set; } = Color.Transparent;

        protected abstract Bitmap GetBitmap();

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            if ((bmpForDraw == null) || (color != Color) || (bmpForDraw.Width != Width) || (bmpForDraw != GetBitmap()))
            {
                bmpForDraw = GetBand(GetBitmap(), Width, WidthCap, TruncateLeft, Color);
                color = Color;
            }

            g.DrawImageUnscaled(bmpForDraw, Left, Top);
        }

        protected static Bitmap GetBand(Bitmap bmpBand, int width, int widthCap, bool truncateLeft, Color color)
        {
            Debug.Assert(width > 0);

            foreach (KeyValuePair<(Bitmap, Color, int), Bitmap> b in cacheBands)
            {
                if ((b.Key.Item1 == bmpBand) && (b.Key.Item2 == color) && (b.Key.Item3 == width))
                    return b.Value;
            }

            Bitmap bmp = PrepareBand(bmpBand, width, widthCap, truncateLeft);
            cacheBands.Add((bmpBand, color, width), bmp);
            if (color != Color.Transparent)
                Utils.LackBitmap(bmp, color);

            return bmp;
        }

        private static Bitmap PrepareBand(Bitmap bmpBand, int width, int widthCap, bool truncateLeft)
        {
            if (!truncateLeft)
            {
                Debug.Assert(width >= bmpBand.Width, $"Width={width}, bmpBand.Width={bmpBand.Width}");
            }

            int widthBody = bmpBand.Width - widthCap - widthCap;
            Debug.Assert(widthBody > 0);
            int offsetX = !truncateLeft || (width >= bmpBand.Width) ? 0 : width - bmpBand.Width + 1;

            Bitmap bmp = new Bitmap(width, bmpBand.Height);
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
    }
}
