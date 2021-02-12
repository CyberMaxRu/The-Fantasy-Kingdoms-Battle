using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Fantasy_Kingdoms_Battle
{
    internal enum ImageState { Normal = 0, Disabled = 1, Over = 2 };
    internal enum ImageModeConversion { Grey, Bright };

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

            // Создаем иконки
            bitmapsNormal = CreateArray(bmp, size);

            if (maxState >= ImageState.Disabled)
                bitmapsDisabled = CreateArray(ConversionBitmap(bmp, ImageModeConversion.Grey), size);

            if (maxState >= ImageState.Over)
                bitmapsOver = CreateArray(ConversionBitmap(bmp, ImageModeConversion.Bright), size);

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

        public BitmapList(BitmapList fromList, int newSize, int borderWidth, Bitmap mask)
        {
            if (mask != null)
            {
                Debug.Assert(mask.Width == newSize);
                Debug.Assert(mask.Height == newSize);
            }

            Debug.Assert(fromList.Size != newSize);

            Size = newSize;
            MaxState = fromList.MaxState;

            bitmapsNormal = new Bitmap[fromList.Count];
            if (MaxState >= ImageState.Disabled)
                bitmapsDisabled = new Bitmap[fromList.Count];

            if (MaxState >= ImageState.Over)
                bitmapsOver = new Bitmap[fromList.Count];

            Rectangle rectSource = new Rectangle(0 + borderWidth, 0 + borderWidth, fromList.Size - (borderWidth * 2), fromList.Size - (borderWidth * 2));
            Rectangle rectTarget = new Rectangle(0, 0, newSize, newSize);

            for (int i = 0; i < fromList.Count; i++)
            {
                Bitmap bmpDest = new Bitmap(newSize, newSize);
                Graphics gDest = Graphics.FromImage(bmpDest);

                gDest.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gDest.SmoothingMode = SmoothingMode.HighQuality;
                gDest.DrawImage(fromList.GetImage(i, ImageState.Normal), rectTarget, rectSource, GraphicsUnit.Pixel);                

                if (mask != null)
                {
                    for (int y = 0; y < bmpDest.Height; y++)
                        for (int x = 0; x < bmpDest.Width; x++)
                        {
                            bmpDest.SetPixel(x, y, Color.FromArgb(bmpDest.GetPixel(x, y).A, bmpDest.GetPixel(x, y)));
                        }
                }

                bitmapsNormal[i] = bmpDest;

                if (MaxState >= ImageState.Disabled)
                    bitmapsDisabled[i] = ConversionBitmap(bmpDest, ImageModeConversion.Grey);
                if (MaxState >= ImageState.Over)
                    bitmapsOver[i] = ConversionBitmap(bmpDest, ImageModeConversion.Bright);

                gDest.Dispose();
            }
        }

        internal int Size { get; }
        internal ImageState MaxState { get; }
        internal int Count { get => bitmapsNormal.Length; }

        internal void Add(Bitmap bmp)
        {
            Array.Resize(ref bitmapsNormal, bitmapsNormal.Length + 1);

            if (bitmapsDisabled != null)
                Array.Resize(ref bitmapsDisabled, bitmapsDisabled.Length + 1);

            if (bitmapsOver != null)
                Array.Resize(ref bitmapsOver, bitmapsOver.Length + 1);

            ReplaceImage(bmp, bitmapsNormal.Length - 1);
        }

        internal void ReplaceImage(Bitmap bmp, int index)
        {
            bitmapsNormal[index] = bmp;

            if (bitmapsDisabled != null)
                bitmapsDisabled[index] = ConversionBitmap(bmp, ImageModeConversion.Grey);

            if (bitmapsOver != null)
                bitmapsOver[index] = ConversionBitmap(bmp, ImageModeConversion.Bright);
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

        internal Bitmap GetImage(int imageIndex, ImageState state)
        {
            Debug.Assert(imageIndex >= 0);
            Debug.Assert(imageIndex < Count);

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

        internal void DrawImage(Graphics g, int imageIndex, ImageState state, int x, int y)
        {
            g.DrawImageUnscaled(GetImage(imageIndex, state), x, y);
        }
    }
}
