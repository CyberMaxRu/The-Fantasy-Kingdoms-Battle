using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Fantasy_King_s_Battle
{
    internal enum ImageState { Normal = 0, Disabled = 1, Over = 2 };

    // Класс - список картинок
    internal sealed class BitmapList
    {
        private Bitmap[] bitmapsNormal;
        private Bitmap[] bitmapsDisabled;
        private Bitmap[] bitmapsOver;

        public BitmapList(string dirResources, string filename, int size, ImageState maxState)
        {
            // Загружаем картинку
            Bitmap bmp = new Bitmap(dirResources + @"Icons\" + filename);

            // Определяем количество иконок
            Debug.Assert(bmp.Width % size == 0);
            Debug.Assert(bmp.Height % size == 0);

            int icons = (bmp.Width / size) * (bmp.Height / size);

            // Создаем иконки
            bitmapsNormal = CreateArray(bmp, size);

            if (maxState >= ImageState.Disabled)
                bitmapsDisabled = CreateArray(GreyBitmap(bmp), size);

            if (maxState >= ImageState.Over)
                bitmapsOver = CreateArray(BrightBitmap(bmp), size);

            // Удаляем исходную картинку - она больше не нужна
            bmp.Dispose();

            Size = size;
            MaxState = maxState;
        }

        public BitmapList(int countIcons, int size, ImageState maxState)
        {
            bitmapsNormal = new Bitmap[countIcons];

            if (maxState >= ImageState.Disabled)
                bitmapsDisabled = new Bitmap[countIcons];

            if (maxState >= ImageState.Over)
                bitmapsOver = new Bitmap[countIcons];

            Size = size;
            MaxState = maxState;
        }

        public BitmapList(BitmapList fromList, int newSize)
        {
            Debug.Assert(fromList.Size != newSize);

            Size = newSize;
            MaxState = fromList.MaxState;

            bitmapsNormal = new Bitmap[fromList.Count];
            if (MaxState >= ImageState.Disabled)
                bitmapsDisabled = new Bitmap[fromList.Count];

            if (MaxState >= ImageState.Over)
                bitmapsOver = new Bitmap[fromList.Count];

            Rectangle rectSource = new Rectangle(0, 0, fromList.Size, fromList.Size);
            Rectangle rectTarget = new Rectangle(0, 0, newSize, newSize);

            for (int x = 0; x < fromList.Count; x++)
            {
                Bitmap bmpDest = new Bitmap(newSize, newSize);
                Graphics gDest = Graphics.FromImage(bmpDest);
                gDest.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gDest.SmoothingMode = SmoothingMode.HighQuality;
                gDest.DrawImage(fromList.GetImage(x, ImageState.Normal), rectTarget, rectSource, GraphicsUnit.Pixel);
                //gDest.DrawImageUnscaled(MaskSmall, 0, 0);
                bitmapsNormal[x] = bmpDest;

                if (MaxState >= ImageState.Disabled)
                    bitmapsDisabled[x] = GreyBitmap(bmpDest);
                if (MaxState >= ImageState.Over)
                    bitmapsOver[x] = BrightBitmap(bmpDest);

                gDest.Dispose();
            }
        }

        internal int Size { get; }
        internal ImageState MaxState { get; }
        internal int Count { get => bitmapsNormal.Length; }

        internal void Add(Bitmap bmp)
        {
            Array.Resize(ref bitmapsNormal, bitmapsNormal.Length + 1);
            bitmapsNormal[bitmapsNormal.Length - 1] = bmp;

            if (bitmapsDisabled != null)
            {
                Array.Resize(ref bitmapsDisabled, bitmapsDisabled.Length + 1);
                bitmapsDisabled[bitmapsDisabled.Length - 1] = GreyBitmap(bmp);
            }

            if (bitmapsOver != null)
            {
                Array.Resize(ref bitmapsOver, bitmapsOver.Length + 1);
                bitmapsOver[bitmapsOver.Length - 1] = BrightBitmap(bmp);
            }
        }

        private Bitmap[] CreateArray(Bitmap bitmap, int size)
        {
            int columns = bitmap.Width / size;
            int lines = bitmap.Height / size;
            Bitmap[] array = new Bitmap[lines * columns];
            Bitmap bmp;
            Graphics g;

            for (int y = 0; y < lines; y++)
                for (int x = 0; x < columns; x++)
                {
                    bmp = new Bitmap(size, size);
                    g = Graphics.FromImage(bmp);
                    g.DrawImage(bitmap, 0, 0, new Rectangle(x * size, y * size, size, size), GraphicsUnit.Pixel);                    
                    g.Dispose();

                    array[y * columns + x] = bmp;
                }

            return array;
        }

        internal static Bitmap GreyBitmap(Bitmap bmp)
        {
            Bitmap output = new Bitmap(bmp.Width, bmp.Height);

            // Перебираем в циклах все пиксели исходного изображения
            for (int j = 0; j < bmp.Height; j++)
                for (int i = 0; i < bmp.Width; i++)
                {
                    // получаем (i, j) пиксель
                    uint pixel = (uint)(bmp.GetPixel(i, j).ToArgb());

                    // получаем компоненты цветов пикселя
                    float R = (pixel & 0x00FF0000) >> 16; // красный
                    float G = (pixel & 0x0000FF00) >> 8; // зеленый
                    float B = pixel & 0x000000FF; // синий
                                                  // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                    R = G = B = (R + G + B) / 3.0f;

                    // собираем новый пиксель по частям (по каналам)
                    uint newPixel = ((uint)bmp.GetPixel(i, j).A << 24) | ((uint)R << 16) | ((uint)G << 8) | ((uint)B);

                    // добавляем его в Bitmap нового изображения
                    output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                }

            return output;
        }

        internal static Bitmap BrightBitmap(Bitmap bmp)
        {
            Bitmap output = new Bitmap(bmp.Width, bmp.Height);

            // Перебираем в циклах все пиксели исходного изображения
            for (int j = 0; j < bmp.Height; j++)
                for (int i = 0; i < bmp.Width; i++)
                {
                    // получаем (i, j) пиксель
                    uint pixel = (uint)(bmp.GetPixel(i, j).ToArgb());

                    // получаем компоненты цветов пикселя
                    float R = (pixel & 0x00FF0000) >> 16; // красный
                    float G = (pixel & 0x0000FF00) >> 8; // зеленый
                    float B = pixel & 0x000000FF; // синий
                                                  // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                    R = Math.Min(R * 1.2f, 255);
                    G = Math.Min(G * 1.2f, 255);
                    B = Math.Min(B * 1.2f, 255);

                    // собираем новый пиксель по частям (по каналам)
                    uint newPixel = ((uint)bmp.GetPixel(i, j).A << 24) | ((uint)R << 16) | ((uint)G << 8) | ((uint)B);

                    // добавляем его в Bitmap нового изображения
                    output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                }

            return output;
        }

        internal Bitmap GetImage(int imageIndex, ImageState state)
        {
            //Debug.Assert(imageIndex >= 0);
            if (imageIndex < 0)
                imageIndex = 0;

            Bitmap bmp;

            switch (state)
            {
                case ImageState.Normal:
                    bmp = bitmapsNormal[imageIndex];
                    break;
                case ImageState.Disabled:
                    bmp = bitmapsDisabled[imageIndex];
                    break;
                case ImageState.Over:
                    bmp = bitmapsOver[imageIndex];
                    break;
                default:
                    throw new Exception("Неизвестное состояние: " + state.ToString());
            }

            Debug.Assert(bmp != null);

            return bmp;
        }
    }
}
