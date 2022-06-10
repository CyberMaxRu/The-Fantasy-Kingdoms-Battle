using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Fantasy_Kingdoms_Battle
{
    // Класс картинки с границами
    internal sealed class BitmapBorder
    {
        private List<(Bitmap,Color)> cacheBitmap = new List<(Bitmap,Color)>();
        
        public BitmapBorder(Bitmap bmpOrigin, bool useCentre, int widthLeftCorner, int widthRightCorner, int heightTopCorner, int heightBottomCorner, 
            int widthHorizBand, int heightTopBand, int heightBottomBand, int heightVertBand, int widthLeftBand, int widthRightBand)
        {
            ArraySides = new Bitmap[3, 3];
            Width = bmpOrigin.Width;
            Height = bmpOrigin.Height;

            Debug.Assert(widthLeftCorner + widthHorizBand + widthRightCorner == bmpOrigin.Width);
            Debug.Assert(heightTopCorner + heightVertBand + heightBottomCorner == bmpOrigin.Height);

            // Режем оригинальный рисунок на картинки с частями границ
            ArraySides[0, 0] = GetImage(0, 0, widthLeftCorner, heightTopCorner);
            ArraySides[0, 1] = GetImage(widthLeftCorner, 0, widthHorizBand, heightTopBand);
            ArraySides[0, 2] = GetImage(widthLeftCorner + widthHorizBand, 0, widthRightCorner, heightTopCorner);
            ArraySides[1, 0] = GetImage(0, heightTopCorner, widthLeftBand, heightVertBand);
            if (useCentre) 
                ArraySides[1, 1] = GetImage(widthLeftCorner, heightTopCorner, widthHorizBand, heightVertBand);
            ArraySides[1, 2] = GetImage(bmpOrigin.Width - widthRightBand, heightTopCorner, widthRightBand, heightVertBand);
            ArraySides[2, 0] = GetImage(0, heightTopCorner + heightVertBand, widthLeftCorner, heightBottomCorner);
            ArraySides[2, 1] = GetImage(widthLeftCorner, bmpOrigin.Height - heightBottomBand, widthHorizBand, heightBottomBand);
            ArraySides[2, 2] = GetImage(widthLeftCorner + widthHorizBand, heightTopCorner + heightVertBand, widthRightCorner, heightBottomCorner);

            bmpOrigin.Dispose();

            Bitmap GetImage(int left, int top, int width, int height)
            {
                Debug.Assert(left + width <= bmpOrigin.Width);
                Debug.Assert(top + height <= bmpOrigin.Height);

                if ((width > 0) && (height > 0))
                {
                    Bitmap b = new Bitmap(width, height);
                    Graphics g = Graphics.FromImage(b);
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.DrawImage(bmpOrigin, new Rectangle(0, 0, width, height), new Rectangle(left, top, width, height), GraphicsUnit.Pixel);
                    g.Dispose();

                    return b;
                }
                else
                    return null;
            }
        }

        internal Bitmap[,] ArraySides;
        internal int Width { get; }
        internal int Height { get; }

        private Bitmap PrepareBorder(int width, int height, Color color)
        {
            //Debug.Assert(width >= widthBorder);
            //Debug.Assert(height >= heightBorder);

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);

            // Левый верхний угол
            g.DrawImageUnscaled(ArraySides[0, 0], 0, 0);

            // Правый верхний угол
            g.DrawImageUnscaled(ArraySides[0, 2], bmp.Width - ArraySides[0, 2].Width, 0);

            // Левый нижний угол
            g.DrawImageUnscaled(ArraySides[2, 0], 0, bmp.Height - ArraySides[2, 0].Height);

            // Правый нижний угол
            g.DrawImageUnscaled(ArraySides[2, 2], bmp.Width - ArraySides[2, 2].Width, bmp.Height - ArraySides[2, 2].Height);

            // Горизонтальные бордюры
            int widthForBand = bmp.Width - ArraySides[0, 0].Width - ArraySides[0, 2].Width;
            int repeats = widthForBand / ArraySides[0, 1].Width;
            int restBorder = widthForBand - (ArraySides[0, 1].Width * repeats);

            for (int i = 0; i < repeats; i++)
            {
                // Верхний бордюр
                g.DrawImageUnscaled(ArraySides[0, 1], ArraySides[0, 0].Width + (i * ArraySides[0, 1].Width), 0);

                // Нижний бордюр
                g.DrawImageUnscaled(ArraySides[2, 1], ArraySides[2, 0].Width + (i * ArraySides[2, 1].Width), bmp.Height - ArraySides[2, 1].Height);
            }

            if (restBorder > 0)
            {
                // Верхний бордюр
                g.DrawImageUnscaledAndClipped(ArraySides[0, 1], new Rectangle(ArraySides[0, 0].Width + (repeats * ArraySides[0, 1].Width), 0, restBorder, ArraySides[0, 1].Height));

                // Нижний бордюр
                g.DrawImageUnscaledAndClipped(ArraySides[2, 1], new Rectangle(ArraySides[2, 0].Width + (repeats * ArraySides[2, 1].Width), bmp.Height - ArraySides[2, 1].Height, restBorder, ArraySides[2, 1].Height));
            }

            // Вертикальные бордюры
            if (ArraySides[1, 0] != null)
            {
                int heightForBand = bmp.Height - ArraySides[0, 0].Height - ArraySides[2, 0].Height;
                repeats = heightForBand / ArraySides[1, 0].Height;
                restBorder = heightForBand - (ArraySides[1, 0].Height * repeats);

                for (int i = 0; i < repeats; i++)
                {
                    // Левый бордюр
                    g.DrawImageUnscaled(ArraySides[1, 0], 0, ArraySides[0, 0].Height + (i * ArraySides[1, 0].Height));

                    // Правый бордюр
                    g.DrawImageUnscaled(ArraySides[1, 2], bmp.Width - ArraySides[1, 2].Width, ArraySides[0, 2].Height + (i * ArraySides[1, 2].Height));
                }

                if (restBorder > 0)
                {
                    // Левый бордюр
                    g.DrawImageUnscaledAndClipped(ArraySides[1, 0], new Rectangle(0, ArraySides[0, 0].Height + (repeats * ArraySides[1, 0].Height), ArraySides[1, 0].Width, restBorder));

                    // Правый бордюр
                    g.DrawImageUnscaledAndClipped(ArraySides[1, 2], new Rectangle(bmp.Width - ArraySides[1, 2].Width, ArraySides[0, 2].Height + (repeats * ArraySides[1, 2].Height), ArraySides[1, 2].Width, restBorder));
                }
            }

            // Середина. Пока просто заполняем черным
            if (ArraySides[1, 1] != null)
            {
                Brush b = new SolidBrush(Color.Black);
                g.FillRectangle(b, new Rectangle(ArraySides[0, 0].Width, ArraySides[0, 0].Height, bmp.Width - ArraySides[0, 0].Width - ArraySides[0, 2].Width, bmp.Height - ArraySides[0, 0].Height - ArraySides[2, 0].Height));
                b.Dispose();
            }

            // Если указан цвет, преобразуем
            Utils.LackBitmap(bmp, color);

            g.Dispose();
            return bmp;
        }

        internal void DrawBorder(Graphics g, int x, int y, int width, int height, Color color)
        {
            Bitmap bmpBorder = null;

            // Ищем бордюр в кэше
            foreach ((Bitmap b, Color c) in cacheBitmap)
                if ((b.Width == width) && (b.Height == height) && (c == color))
                {
                    bmpBorder = b;
                    break;
                }

            if (bmpBorder is null)
            {
                bmpBorder = PrepareBorder(width, height, color);
                cacheBitmap.Add((bmpBorder, color));
            }

            g.DrawImageUnscaled(bmpBorder, x, y);
        }
    }
}
