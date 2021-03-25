using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Fantasy_Kingdoms_Battle
{
    internal enum ImageModeConversion { Grey, Bright };

    // Класс - список картинок
    internal sealed class BitmapList
    {
        private Bitmap[] bitmapsNormal;
        private Bitmap[] bitmapsDisabled;
        private Bitmap[] bitmapsNormalOver;
        private Bitmap[] bitmapsDisabledOver;

        public BitmapList(Bitmap bmp, int size, bool withDisabled, bool withOver)
        {
            Size = size;
            WithDisabled = withDisabled;
            WithOver = withOver;

            // Определяем количество иконок
            Debug.Assert(bmp.Width % size == 0);
            Debug.Assert(bmp.Height % size == 0);

            // Создаем иконки
            bitmapsNormal = CreateArray(bmp, size);
            if (withOver)
                bitmapsNormalOver = CreateArray(ConversionBitmap(bmp, ImageModeConversion.Bright), size);

            if (WithDisabled)
            {
                Bitmap bmpDisabled = ConversionBitmap(bmp, ImageModeConversion.Grey);
                bitmapsDisabled = CreateArray(bmpDisabled, size);

                if (withOver)
                    bitmapsDisabledOver = CreateArray(ConversionBitmap(bmpDisabled, ImageModeConversion.Bright), size);

                bmpDisabled.Dispose();
            }

            // Удаляем исходную картинку - она больше не нужна
            bmp.Dispose();
        }

        public BitmapList(int countIcons, int size, bool withDisabled, bool withOver)
        {
            Size = size;
            WithDisabled = withDisabled;
            WithOver = withOver;
            CreateArrays(countIcons);
        }

        public BitmapList(BitmapList fromList, int newSize, int borderWidth, Bitmap mask)
        {
            if (mask != null)
            {
                Debug.Assert(mask.Width == newSize);
                Debug.Assert(mask.Height == newSize);
            }

            Debug.Assert(newSize < fromList.Size);

            Size = newSize;
            WithDisabled = fromList.WithDisabled;
            WithOver = fromList.WithOver;
            CreateArrays(fromList.Count);

            for (int i = 0; i < fromList.Count; i++)
            {
                ReplaceImageWithResize(fromList, i, borderWidth, mask);
            }
        }

        internal int Size { get; }
        internal bool WithDisabled { get; set; }
        internal bool WithOver { get; set; }
        internal int Count { get => bitmapsNormal.Length; }

        internal void ReplaceImageWithResize(BitmapList fromList, int idx, int borderWidth, Bitmap mask)
        {
            if (mask != null)
            {
                Debug.Assert(mask.Width == Size);
                Debug.Assert(mask.Height == Size);
            }

            bitmapsNormal?[idx]?.Dispose();
            bitmapsNormalOver?[idx]?.Dispose();
            bitmapsDisabled?[idx]?.Dispose();
            bitmapsDisabledOver?[idx]?.Dispose();

            Rectangle rectSource = new Rectangle(0 + borderWidth, 0 + borderWidth, fromList.Size - (borderWidth * 2), fromList.Size - (borderWidth * 2));
            Rectangle rectTarget = new Rectangle(0, 0, Size, Size);

            Bitmap bmpDest = new Bitmap(Size, Size);
            Graphics gDest = Graphics.FromImage(bmpDest);

            gDest.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gDest.SmoothingMode = SmoothingMode.HighQuality;
            gDest.DrawImage(fromList.GetImage(idx, true, false), rectTarget, rectSource, GraphicsUnit.Pixel);

            if (mask != null)
            {
                for (int y = 0; y < bmpDest.Height; y++)
                    for (int x = 0; x < bmpDest.Width; x++)
                    {
                        bmpDest.SetPixel(x, y, Color.FromArgb(mask.GetPixel(x, y).A, bmpDest.GetPixel(x, y)));
                    }
            }

            bitmapsNormal[idx] = bmpDest;
            if (WithOver)
                bitmapsNormalOver[idx] = ConversionBitmap(bmpDest, ImageModeConversion.Bright);

            if (WithDisabled)
            {
                bitmapsDisabled[idx] = ConversionBitmap(bmpDest, ImageModeConversion.Grey);
                if (WithOver)
                    bitmapsDisabledOver[idx] = ConversionBitmap(bitmapsDisabled[idx], ImageModeConversion.Bright);
            }

            gDest.Dispose();
        }

        internal void Add(Bitmap bmp)
        {
            Array.Resize(ref bitmapsNormal, bitmapsNormal.Length + 1);

            if (bitmapsNormalOver != null)
                Array.Resize(ref bitmapsNormalOver, bitmapsNormalOver.Length + 1);

            if (bitmapsDisabled != null)
                Array.Resize(ref bitmapsDisabled, bitmapsDisabled.Length + 1);

            if (bitmapsDisabledOver != null)
                Array.Resize(ref bitmapsDisabledOver, bitmapsDisabledOver.Length + 1);

            ReplaceImage(bmp, bitmapsNormal.Length - 1);
        }

        internal void ReplaceImage(Bitmap bmp, int index)
        {
            bitmapsNormal[index] = bmp;

            if (bitmapsNormalOver != null)
                bitmapsNormalOver[index] = ConversionBitmap(bmp, ImageModeConversion.Bright);

            if (bitmapsDisabled != null)
                bitmapsDisabled[index] = ConversionBitmap(bmp, ImageModeConversion.Grey);

            if (bitmapsDisabledOver != null)
                bitmapsDisabledOver[index] = ConversionBitmap(bitmapsDisabled[index], ImageModeConversion.Bright);
        }

        private void CreateArrays(int countIcons)
        {
            bitmapsNormal = new Bitmap[countIcons];
            if (WithOver)
                bitmapsNormalOver = new Bitmap[countIcons];

            if (WithDisabled)
            {
                bitmapsDisabled = new Bitmap[countIcons];

                if (WithOver)
                    bitmapsDisabledOver = new Bitmap[countIcons];
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

        internal Bitmap ConversionBitmap(Bitmap bmp, ImageModeConversion mode)
        {
            Bitmap output = new Bitmap(bmp);
            byte newColor0, newColor1, newColor2;

            Rectangle rect = new Rectangle(0, 0, output.Width, output.Height);
            BitmapData bmpData = output.LockBits(rect, ImageLockMode.ReadWrite, output.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * output.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Byte 0: Blue, Byte 1: Green, Byte 2: Red, Byte 3: Alpha
            for (int counter = 0; counter < rgbValues.Length; counter += 4)
            {
                switch (mode)
                {
                    case ImageModeConversion.Grey:
                        newColor0 = newColor1 = newColor2 = Convert.ToByte((rgbValues[counter + 0] + rgbValues[counter + 1] + rgbValues[counter + 2]) / 3);
                        break;
                    case ImageModeConversion.Bright:
                        newColor0 = Convert.ToByte(Math.Min(rgbValues[counter + 0] * 1.2f, 255));
                        newColor1 = Convert.ToByte(Math.Min(rgbValues[counter + 1] * 1.2f, 255));
                        newColor2 = Convert.ToByte(Math.Min(rgbValues[counter + 2] * 1.2f, 255));
                        break;
                    default:
                        throw new Exception("Неизвестный режим: " + mode.ToString()); 
                }

                rgbValues[counter + 0] = newColor0;
                rgbValues[counter + 1] = newColor1;
                rgbValues[counter + 2] = newColor2;
            }

            Marshal.Copy(rgbValues, 0, ptr, bytes);
            output.UnlockBits(bmpData);

            return output;
        }

        internal Bitmap GetImage(int imageIndex, bool enabled, bool over)
        {
            Debug.Assert(imageIndex >= 0);
            Debug.Assert(imageIndex < Count);

            Bitmap[] array;
            if (enabled)
                array = over ? bitmapsNormalOver : bitmapsNormal;
            else
                array = over ? bitmapsDisabledOver : bitmapsDisabled;

            Debug.Assert(array[imageIndex] != null);
            return array[imageIndex];
        }

        internal void DrawImage(Graphics g, int imageIndex, bool enabled, bool over, int x, int y)
        {
            g.DrawImageUnscaled(GetImage(imageIndex, enabled, over), x, y);
        }
    }
}
