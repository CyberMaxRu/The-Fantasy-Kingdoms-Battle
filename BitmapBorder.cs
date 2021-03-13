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
        private Bitmap[,] arraySides;
        private int widthBorder;
        private int heightBorder;

        public BitmapBorder(string filename, int widthLeftCorner, int widthRightCorner, int heightTopCorner, int heightBottomCorner, 
            int widthHorizBand, int heightTopBand, int heightBottomBand, int heightVertBand, int widthLeftBand, int widthRightBand)
        {
            arraySides = new Bitmap[3, 3];
            Bitmap bmpOrigin = new Bitmap(filename);
            widthBorder = bmpOrigin.Width;
            heightBorder = bmpOrigin.Height;

            Debug.Assert(widthLeftCorner + widthHorizBand + widthRightCorner == bmpOrigin.Width);
            Debug.Assert(heightTopCorner + heightVertBand + heightBottomCorner == bmpOrigin.Height);

            // Режем оригинальный рисунок на картинки с частями границ
            arraySides[0, 0] = GetImage(0, 0, widthLeftCorner, heightTopCorner);
            arraySides[0, 1] = GetImage(widthLeftCorner, 0, widthHorizBand, heightTopBand);
            arraySides[0, 2] = GetImage(widthLeftCorner + widthHorizBand, 0, widthRightCorner, heightTopCorner);
            arraySides[1, 0] = GetImage(0, heightTopCorner, widthLeftBand, heightVertBand);
            arraySides[1, 2] = GetImage(bmpOrigin.Width - widthRightBand, heightTopCorner, widthRightBand, heightVertBand);
            arraySides[2, 0] = GetImage(0, heightTopCorner + heightVertBand, widthLeftCorner, heightBottomCorner);
            arraySides[2, 1] = GetImage(widthLeftCorner, bmpOrigin.Height - heightBottomBand, widthHorizBand, heightBottomBand);
            arraySides[2, 2] = GetImage(widthLeftCorner + widthHorizBand, heightTopCorner + heightVertBand, widthRightCorner, heightBottomCorner);

            bmpOrigin.Dispose();

            Bitmap GetImage(int left, int top, int width, int height)
            {
                Debug.Assert(left + width <= bmpOrigin.Width);
                Debug.Assert(top + height <= bmpOrigin.Height);

                Bitmap b = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(b);
                g.CompositingMode = CompositingMode.SourceCopy;
                g.DrawImage(bmpOrigin, new Rectangle(0, 0, width, height), new Rectangle(left, top, width, height), GraphicsUnit.Pixel);
                g.Dispose();

                return b;
            }
        }

        internal Bitmap DrawBorder(int width, int height)
        {
            Debug.Assert(width >= widthBorder);
            Debug.Assert(height >= heightBorder);

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);

            // Левый верхний угол
            g.DrawImageUnscaled(arraySides[0, 0], 0, 0);

            // Правый верхний угол
            g.DrawImageUnscaled(arraySides[0, 2], bmp.Width - arraySides[0, 2].Width, 0);

            // Левый нижний угол
            g.DrawImageUnscaled(arraySides[2, 0], 0, bmp.Height - arraySides[2, 0].Height);

            // Правый нижний угол
            g.DrawImageUnscaled(arraySides[2, 2], bmp.Width - arraySides[2, 2].Width, bmp.Height - arraySides[2, 2].Height);

            // Горизонтальные бордюры
            int widthForBand = bmp.Width - arraySides[0, 0].Width - arraySides[0, 2].Width;
            int repeats = widthForBand / arraySides[0, 1].Width;
            int restBorder = widthForBand - (arraySides[0, 1].Width * repeats);

            for (int i = 0; i < repeats; i++)
            {
                // Верхний бордюр
                g.DrawImageUnscaled(arraySides[0, 1], arraySides[0, 0].Width + (i * arraySides[0, 1].Width), 0);

                // Нижний бордюр
                g.DrawImageUnscaled(arraySides[2, 1], arraySides[2, 0].Width + (i * arraySides[2, 1].Width), bmp.Height - arraySides[2, 1].Height);
            }

            // Верхний бордюр
            g.DrawImageUnscaledAndClipped(arraySides[0, 1], new Rectangle(arraySides[0, 0].Width + (repeats * arraySides[0, 1].Width), 0, restBorder, arraySides[0, 1].Height));

            // Нижний бордюр
            g.DrawImageUnscaledAndClipped(arraySides[2, 1], new Rectangle(arraySides[2, 0].Width + (repeats * arraySides[2, 1].Width), bmp.Height - arraySides[2, 1].Height, restBorder, arraySides[2, 1].Height));

            // Вертикальные бордюры
            int heightForBand = bmp.Height - arraySides[0, 0].Height - arraySides[2, 0].Height;
            repeats = heightForBand / arraySides[1, 0].Height;
            restBorder = heightForBand - (arraySides[1, 0].Height * repeats);

            for (int i = 0; i < repeats; i++)
            {
                // Левый бордюр
                g.DrawImageUnscaled(arraySides[1, 0], 0, arraySides[0, 0].Height + (i * arraySides[1, 0].Height));

                // Правый бордюр
                g.DrawImageUnscaled(arraySides[1, 2], bmp.Width - arraySides[1, 2].Width, arraySides[0, 2].Height + (i * arraySides[1, 2].Height));
            }

            // Левый бордюр
            g.DrawImageUnscaledAndClipped(arraySides[1, 0], new Rectangle(0, arraySides[0, 0].Height + (repeats * arraySides[1, 0].Height), arraySides[1, 0].Width, restBorder));

            // Правый бордюр
            g.DrawImageUnscaledAndClipped(arraySides[1, 2], new Rectangle(bmp.Width - arraySides[1, 2].Width, arraySides[0, 2].Height + (repeats * arraySides[1, 2].Height), arraySides[1, 2].Width, restBorder));

            g.Dispose();
            return bmp;
        }
    }
}
