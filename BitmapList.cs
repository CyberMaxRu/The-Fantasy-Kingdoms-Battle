using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Fantasy_Kingdoms_Battle
{
    // Список картинок
    internal sealed class BitmapList
    {
        private enum ImageModeConversion { Grey, Bright };

        private readonly List<Bitmap> listBitmapNormal;
        private readonly List<Bitmap> listBitmapDisabled;
        private readonly List<Bitmap> listBitmapNormalOver;
        private readonly List<Bitmap> listBitmapDisabledOver;

        public BitmapList(Bitmap bmp, Size size, bool withDisabled, bool withOver)
        {
            Debug.Assert(bmp.Width % size.Width == 0);
            Debug.Assert(bmp.Height % size.Height == 0);

            Size = size;
            WithDisabled = withDisabled;
            WithOver = withOver;

            // Создаем иконки
            listBitmapNormal = new List<Bitmap>();

            if (WithOver)
                listBitmapNormalOver = new List<Bitmap>();

            if (WithDisabled)
                listBitmapDisabled = new List<Bitmap>();

            if (WithDisabled && WithOver)
                listBitmapDisabledOver = new List<Bitmap>();

            //
            AddBitmap(bmp);
        }

        public BitmapList(BitmapList fromList, Size newSize, int borderWidth, Bitmap mask)
        {
            if (mask != null)
            {
                Debug.Assert(mask.Width == newSize.Width);
                Debug.Assert(mask.Height == newSize.Height);
            }
            Debug.Assert(newSize.Width < fromList.Size.Width);
            Debug.Assert(newSize.Height < fromList.Size.Height);

            Size = newSize;
            WithDisabled = fromList.WithDisabled;
            WithOver = fromList.WithOver;

            listBitmapNormal = new List<Bitmap>(fromList.Count);

            if (WithOver)
                listBitmapNormalOver = new List<Bitmap>(fromList.Count);

            if (WithDisabled)
                listBitmapDisabled = new List<Bitmap>(fromList.Count);

            if (WithDisabled && WithOver)
                listBitmapDisabledOver = new List<Bitmap>(fromList.Count);

            for (int i = 0; i < fromList.Count; i++)
            {
                AddNullImage();
                ReplaceImageWithResize(fromList, i, borderWidth, mask);
            }
        }

        internal Size Size { get; }
        internal bool WithDisabled { get; set; }
        internal bool WithOver { get; set; }
        internal int Count { get => listBitmapNormal.Count; }

        internal void ReplaceImageWithResize(BitmapList fromList, int idx, int borderWidth, Bitmap mask)
        {
            if (mask != null)
            {
                Debug.Assert(mask.Width == Size.Width);
                Debug.Assert(mask.Height == Size.Height);
            }

            listBitmapNormal?[idx]?.Dispose();
            listBitmapNormalOver?[idx]?.Dispose();
            listBitmapDisabled?[idx]?.Dispose();
            listBitmapDisabledOver?[idx]?.Dispose();

            Rectangle rectSource = new Rectangle(0 + borderWidth, 0 + borderWidth, fromList.Size.Width - (borderWidth * 2), fromList.Size.Height - (borderWidth * 2));
            Rectangle rectTarget = new Rectangle(0, 0, Size.Width, Size.Height);

            Bitmap bmpDest = new Bitmap(Size.Width, Size.Height);
            Graphics gDest = Graphics.FromImage(bmpDest);

            gDest.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gDest.SmoothingMode = SmoothingMode.HighQuality;
            Bitmap b = fromList.GetImage(idx, true, false);
            if (b != null)
            {
                gDest.DrawImage(b, rectTarget, rectSource, GraphicsUnit.Pixel);

                if (mask != null)
                {
                    for (int y = 0; y < bmpDest.Height; y++)
                        for (int x = 0; x < bmpDest.Width; x++)
                        {
                            bmpDest.SetPixel(x, y, Color.FromArgb(mask.GetPixel(x, y).A, bmpDest.GetPixel(x, y)));
                        }
                }

                listBitmapNormal[idx] = bmpDest;
                if (WithOver)
                    listBitmapNormalOver[idx] = ConversionBitmap(bmpDest, ImageModeConversion.Bright);

                if (WithDisabled)
                {
                    listBitmapDisabled[idx] = ConversionBitmap(bmpDest, ImageModeConversion.Grey);
                    if (WithOver)
                        listBitmapDisabledOver[idx] = ConversionBitmap(listBitmapDisabled[idx], ImageModeConversion.Bright);
                }
            }

            gDest.Dispose();
        }

        private void AddNullImage()
        {
            listBitmapNormal?.Add(null);
            listBitmapNormalOver?.Add(null);
            listBitmapDisabled?.Add(null);
            listBitmapDisabledOver?.Add(null);
        }

        internal void AddEmptySlots(int count)
        {
            listBitmapNormal.Capacity = listBitmapNormal.Count + count;
            listBitmapNormalOver.Capacity = listBitmapNormalOver.Count + count;
            listBitmapDisabled.Capacity = listBitmapDisabled.Count + count;
            listBitmapDisabledOver.Capacity = listBitmapDisabledOver.Count + count;

            for (int i = 0; i < count; i++)
            {
                AddNullImage();
                //ReplaceImage(new Bitmap(Size, Size), listBitmapNormal.Count - 1);
            }
        }

        internal void AddBitmap(Bitmap bmp)
        {
            CreateArray(listBitmapNormal, bmp);

            if (WithOver)
                CreateArray(listBitmapNormalOver, ConversionBitmap(bmp, ImageModeConversion.Bright));

            if (WithDisabled)
            {
                using (Bitmap bmpDisabled = ConversionBitmap(bmp, ImageModeConversion.Grey))
                {
                    CreateArray(listBitmapDisabled, bmpDisabled);

                    if (WithOver)
                        CreateArray(listBitmapDisabledOver, ConversionBitmap(bmpDisabled, ImageModeConversion.Bright));
                }
            }

            // Удаляем исходную картинку - она больше не нужна
            bmp.Dispose();
        }

        internal void ReplaceImage(Bitmap bmp, int index)
        {
            listBitmapNormal[index] = bmp;

            if (listBitmapNormalOver != null)
                listBitmapNormalOver[index] = ConversionBitmap(bmp, ImageModeConversion.Bright);

            if (listBitmapDisabled != null)
                listBitmapDisabled[index] = ConversionBitmap(bmp, ImageModeConversion.Grey);

            if (listBitmapDisabledOver != null)
                listBitmapDisabledOver[index] = ConversionBitmap(listBitmapDisabled[index], ImageModeConversion.Bright);
        }

        private void CreateArray(List<Bitmap> list, Bitmap bitmap)
        {
            int columns = bitmap.Width / Size.Width;
            int lines = bitmap.Height / Size.Height;
            list.Capacity = list.Capacity + columns * lines;
            Bitmap bmp;
            Graphics g;
            
            for (int y = 0; y < lines; y++)
                for (int x = 0; x < columns; x++)
                {
                    bmp = new Bitmap(Size.Width, Size.Height);
                    using (g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(bitmap, 0, 0, new Rectangle(x * Size.Width, y * Size.Height, Size.Width, Size.Height), GraphicsUnit.Pixel);
                    }

                    list.Add(bmp);
                }
        }

        private Bitmap ConversionBitmap(Bitmap bmp, ImageModeConversion mode)
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
            Debug.Assert(imageIndex < Count, $"Попытка взять ImageIndex = {imageIndex} из коллекции в {Count}");

            List<Bitmap> array;
            if (enabled)
                array = over ? listBitmapNormalOver : listBitmapNormal;
            else
                array = over ? listBitmapDisabledOver : listBitmapDisabled;

            return array[imageIndex];
        }

        internal void DrawImage(Graphics g, int imageIndex, bool enabled, bool over, int x, int y)
        {
            g.DrawImageUnscaled(GetImage(imageIndex, enabled, over), x, y);
        }

        internal void NullFromIndex(int fromIndex, int count)
        {
            ClearArray(listBitmapNormal);
            ClearArray(listBitmapNormalOver);
            ClearArray(listBitmapDisabled);
            ClearArray(listBitmapDisabledOver);

            void ClearArray(List<Bitmap> arr)
            {
                if (arr != null)
                {
                    for (int i = fromIndex; i < fromIndex + count; i++)
                    {
                        if (arr[i] != null)
                        {
                            arr[i].Dispose();
                            arr[i] = null;
                        }
                    }
                }
            }
        }
    }
}
