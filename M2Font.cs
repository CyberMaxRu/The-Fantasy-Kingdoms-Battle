﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class M2Font
    {
        internal Bitmap[] symbols;
        internal int maxWidthSymbol;

        public M2Font(string filename)
        {
            Bitmap bmpFonts = LoadBitmap(filename + ".png", "Fonts");

            // Загружаем tuv-файл
            string file = File.ReadAllText(Program.FolderResources + @"Fonts\" + filename + ".tuv", Encoding.GetEncoding(1251));
            string [] conf = file.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int count = Convert.ToInt32(conf[1].Substring(9));

            symbols = new Bitmap[count];
            maxWidthSymbol = 0;
            MaxHeightSymbol = 0;

            // Получаем конфигурацию расположения символов
            int left, leftAndWidth, top, topAndHeight, width, height;
            string line;
            Bitmap sym;
            Graphics g;
            for (int i = 0; i < count; i++)
            {
                line = conf[i + 3];
                left = GetValue() - 1;// Делаем смещение влево на 1, т.к. отсчет в файле с 1
                top = GetValue() - 1;
                leftAndWidth = GetValue();
                topAndHeight = GetValue();

                // Если есть изображение слева или справа от меток, учитываем его
                /*for (int j = top; j < topAndHeight; j++)
                {
                    if (bmpFonts.GetPixel(left - 1, j).A > 0)
                    {
                        left--;
                        leftAndWidth--;
                        break;
                    }
                }
                for (int j = top; j < topAndHeight; j++)
                    if (bmpFonts.GetPixel(leftAndWidth + 1, j).A > 0)
                    {
                        leftAndWidth++;
                        break;
                    }*/

                width = leftAndWidth - left;
                height = topAndHeight - top;

                maxWidthSymbol = Math.Max(maxWidthSymbol, width);
                MaxHeightSymbol = Math.Max(MaxHeightSymbol, height);

                // Создаем картинку буквы
                sym = new Bitmap(width, height);
                g = Graphics.FromImage(sym);
                g.DrawImage(bmpFonts, 0, 0, new Rectangle(left, top, width, height), GraphicsUnit.Pixel);
                g.Dispose();

                // Это проверка, что не отрезается правая часть символа
                /*
                for (int j = top; j < topAndHeight; j++)
                {
                    if (bmpFonts.GetPixel(leftAndWidth, j).A == 255)
                    {
                        if (!System.IO.Directory.Exists(@"f:\symbols\" + filename + "\\"))
                            System.IO.Directory.CreateDirectory(@"f:\symbols\" + filename + "\\");
                        sym.Save(@"f:\symbols\" + filename + "\\" + i.ToString() + " error.png");

                        //throw new Exception($"{i} R: {bmpFonts.GetPixel(leftAndWidth, j).R} G: {bmpFonts.GetPixel(leftAndWidth, j).G} B: {bmpFonts.GetPixel(leftAndWidth, j).B} A: {bmpFonts.GetPixel(leftAndWidth, j).A}"
                        //    + $" X = {leftAndWidth} Y = {j}");
                    }
                }*/

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

        internal int MaxHeightSymbol { get; }

        internal Bitmap GetBitmap(string text, Color color)
        {
            Debug.Assert(text.Length > 0);
            Debug.Assert(MaxHeightSymbol > 0);

            // Сначала создаем картинку с максимальным размером, который может быть
            Bitmap bmpRaw = new Bitmap(maxWidthSymbol * text.Length, MaxHeightSymbol);

            // Рисуем текст на предварительной картинке
            Bitmap bmpSymbol;
            int left = 0;
            byte[] text1251 = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1251), Encoding.UTF8.GetBytes(text));

            bool defaultColor = true;
            Graphics gRaw = Graphics.FromImage(bmpRaw);

            // Сначала проходим по тексту в поисках "}"
            // Если нет открывающей скобки, значит, 99%, она была на предыдущей строке
            // Поэтому сразу окрашивает в другой цвет
            foreach (byte b in text1251)
            {
                if (b == '{')
                    break;
                else if (b == '}')
                {
                    defaultColor = false;
                    break;
                }
            }

            foreach (byte b in text1251)
            {
                if (b == '{')
                {
                    Debug.Assert(defaultColor);
                    defaultColor = false;
                    continue;
                }
                else if (b == '}')
                {
                    Debug.Assert(!defaultColor);
                    defaultColor = true;
                    continue;
                }

                bmpSymbol = symbols[b - 32];
                DrawSymbol(gRaw, bmpSymbol, left, defaultColor ? color : Color.Yellow);
                left += bmpSymbol.Width - 1;// В M2 символы рисуются со смещением в 1 влево, чтобы пустая вертикальная черта с альфа-каналом накладывалась на символ слева
            }
            Debug.Assert(left <= bmpRaw.Width);
            gRaw.Dispose();
            //if (text == "2000")
            //    bmpRaw.Save(@"f:\symbols\_ico.png", System.Drawing.Imaging.ImageFormat.Png);

            // Зная фактический размер текста, переносим его на новую картинку с правильным размером
            Bitmap bmpResult = new Bitmap(left + 1, MaxHeightSymbol);
            Graphics gResult = Graphics.FromImage(bmpResult);
            gResult.DrawImageUnscaled(bmpRaw, 0, 0);
            gResult.Dispose();
            bmpRaw.Dispose();

            return bmpResult;

            void DrawSymbol(Graphics g, Bitmap bmpOrigSymbol, int leftForDraw, Color colorSymbol)
            {
                // Создаем копию картинки с символом
                Bitmap bmpTemp = new Bitmap(bmpOrigSymbol);

                // Применяем указанный цвет
                Rectangle rect = new Rectangle(0, 0, bmpTemp.Width, bmpTemp.Height);
                BitmapData bmpData = bmpTemp.LockBits(rect, ImageLockMode.ReadWrite, bmpTemp.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int bytes = Math.Abs(bmpData.Stride) * bmpTemp.Height;
                byte[] rgbValues = new byte[bytes];
                Marshal.Copy(ptr, rgbValues, 0, bytes);

                for (int counter = 0; counter < rgbValues.Length; counter += 4)
                {
                    if (rgbValues[counter + 3] > 0)
                    {
                        rgbValues[counter + 0] = Convert.ToByte(rgbValues[counter + 0] * colorSymbol.B / 255);
                        rgbValues[counter + 1] = Convert.ToByte(rgbValues[counter + 1] * colorSymbol.G / 255);
                        rgbValues[counter + 2] = Convert.ToByte(rgbValues[counter + 2] * colorSymbol.R / 255);
                    }
                }

                Marshal.Copy(rgbValues, 0, ptr, bytes);
                bmpTemp.UnlockBits(bmpData);

                // Рисуем символ
                g.DrawImageUnscaled(bmpTemp, leftForDraw, 0);

                bmpTemp.Dispose();
            }
        }

        internal static string StripText(string text)
        {
            return text.Replace("{", "").Replace("}", "");
        }

        internal int WidthText(string text)
        {
            int width = 0;
            byte[] text1251 = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1251), Encoding.UTF8.GetBytes(StripText(text)));

            foreach (byte b in text1251)
            {
                width += symbols[b - 32].Width - 1;
            }

            return width + 1;
        }
    }
}