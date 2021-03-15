using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class M2Font
    {
        internal Bitmap[] symbols;
        internal int maxWidthSymbol;
        internal int maxHeightSymbol;
        public M2Font(string dirResources, string filename)
        {
            Bitmap bmpFonts = new Bitmap(dirResources + @"Fonts\" + filename + ".png");
            Debug.Assert(bmpFonts.HorizontalResolution >= 95);
            Debug.Assert(bmpFonts.HorizontalResolution <= 96);
            Debug.Assert(bmpFonts.VerticalResolution >= 95);
            Debug.Assert(bmpFonts.VerticalResolution <= 96);

            // Загружаем tuv-файл
            string file = File.ReadAllText(dirResources + @"Fonts\" + filename + ".tuv", Encoding.GetEncoding(1251));
            string [] conf = file.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int count = Convert.ToInt32(conf[1].Substring(9));

            symbols = new Bitmap[count];
            maxWidthSymbol = 0;
            maxHeightSymbol = 0;

            // Получаем конфигурацию расположения символов
            int left, leftAndWidth, top, topAndHeight, width, height;
            string line;
            Bitmap sym;
            Graphics g;
            for (int i = 0; i < count; i++)
            {
                line = conf[i + 3];
                left = GetValue();// Делаем смещение влево на 1, т.к. у некоторых букв начало смещено на 1 пиксел влево
                top = GetValue();
                leftAndWidth = GetValue();
                topAndHeight = GetValue();
                //if (i == 18)
                //{}

                // Если есть изображение слева или справа от меток, учитываем его
                for (int j = top; j < topAndHeight; j++)
                {
                    if (bmpFonts.GetPixel(left - 1, j).A > 0)
                    {
                        left--;
                        break;
                    }
                }
                for (int j = top; j < topAndHeight; j++)
                    if (bmpFonts.GetPixel(leftAndWidth + 1, j).A > 0)
                    {
                        leftAndWidth++;
                        break;
                    }

                width = leftAndWidth - left + 1;
                height = topAndHeight - top + 1;

                maxWidthSymbol = Math.Max(maxWidthSymbol, width);
                maxHeightSymbol = Math.Max(maxHeightSymbol, height);

                // Создаем картинку буквы
                sym = new Bitmap(width, height);
                g = Graphics.FromImage(sym);
                g.DrawImage(bmpFonts, 0, 0, new Rectangle(left, top, width, height), GraphicsUnit.Pixel);
                g.Dispose();
                /*if (filename == "small_c")
                {
                    sym.Save(@"f:\symbols\" + i.ToString() + ".png");

                    if (i == 137)
                    {

                    }
                }*/

                symbols[i] = sym;
            }

            bmpFonts.Dispose();

            int GetValue()
            {
                string v;
                if (line.IndexOf('\t') != -1)
                {
                    v = line.Substring(0, line.IndexOf('\t'));
                    line = line.Substring(v.Length + 1);
                }
                else
                    v = line;

                return Convert.ToInt32(v);
            }
        }

        internal int MaxHeightSymbol { get => maxHeightSymbol; }

        internal Bitmap GetBitmap(string text, Color color)
        {
            Debug.Assert(text.Length > 0);

            // Сначала создаем картинку с максимальным размером, который может быть
            Bitmap bmpRaw = new Bitmap(maxWidthSymbol * text.Length, maxHeightSymbol);

            // Рисуем текст на предварительной картинке
            Bitmap bmpSymbol;
            int left = 0;
            byte[] text1251 = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1251), Encoding.UTF8.GetBytes(text));

            Graphics gRaw = Graphics.FromImage(bmpRaw);
            foreach (byte b in text1251)
            {
                bmpSymbol = symbols[b - 32];
                gRaw.DrawImageUnscaled(bmpSymbol, left, 0);
                left += bmpSymbol.Width - 1;
            }
            Debug.Assert(left <= bmpRaw.Width);
            gRaw.Dispose();
            //if (text == "2000")
            //    bmpRaw.Save(@"f:\symbols\_ico.png", System.Drawing.Imaging.ImageFormat.Png);

            // Зная фактический размер текста, переносим его на новую картинку с правильным размером
            Bitmap bmpResult = new Bitmap(left, maxHeightSymbol);
            Graphics gResult = Graphics.FromImage(bmpResult);
            gResult.DrawImageUnscaled(bmpRaw, 0, 0);
            gResult.Dispose();
            bmpRaw.Dispose();

            // Применяем указанный цвет
            Rectangle rect = new Rectangle(0, 0, bmpResult.Width, bmpResult.Height);
            BitmapData bmpData = bmpResult.LockBits(rect, ImageLockMode.ReadWrite, bmpResult.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmpResult.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (int counter = 0; counter < rgbValues.Length; counter += 4)
            {
                rgbValues[counter + 0] = Convert.ToByte(rgbValues[counter + 0] * color.B / 255);
                rgbValues[counter + 1] = Convert.ToByte(rgbValues[counter + 1] * color.G / 255);
                rgbValues[counter + 2] = Convert.ToByte(rgbValues[counter + 2] * color.R / 255);
            }

            Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmpResult.UnlockBits(bmpData);

            return bmpResult;
        }

        internal int WidthText(string text)
        {
            int width = 0;
            byte[] text1251 = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1251), Encoding.UTF8.GetBytes(text));

            foreach (byte b in text1251)
            {
                width += symbols[b - 32].Width - 1;
            }

            return width;
        }
    }
}