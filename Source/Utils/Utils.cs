﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

namespace Fantasy_Kingdoms_Battle
{
    // Класс целого числа с дробной частью с тремя знаками
    internal sealed class Integer1000
    {
        private int _value;

        public Integer1000()
        {
            Value = 0;
        }

        public Integer1000(int asInteger)
        {
            Value = asInteger * 1000;
        }

        internal int Value { get => _value; set { _value = value; AsInteger = value / 1000; } }
        internal int AsInteger { get; private set; }
        public override string ToString() => AsInteger.ToString();
    }

    // Класс вспомогательных методов
    internal sealed class Utils
    {
        private static readonly float dpiX;
        private static readonly float dpiY;
        private static readonly int DEFAULT_DPI = 96;

        static Utils()
        {
            // Определяем DPI для корректировки картинок
            Graphics gDpi = Graphics.FromHwnd(IntPtr.Zero);
            dpiX = gDpi.DpiX;
            dpiY = gDpi.DpiY;
            gDpi.Dispose();
        }

        internal static double DistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        internal static void TrimActions(List<DateTime> list)
        {
            DateTime currentDateTime = DateTime.Now;

            TimeSpan diffTime;
            for (int i = 0; i < list.Count; i++)
            {
                diffTime = currentDateTime - list[i];
                if (diffTime.TotalMilliseconds <= 1_000)
                {
                    list.RemoveRange(0, i);
                    break;
                }
            }
        }

        internal static bool PointInRectagle(int left, int top, int width, int height, int x, int y)
        {
            return (left <= x) && (top <= y) && (left + width >= x) && (top + height >= y);
        }

        internal static string FormatGreatness(int add, int perDay)
        {
            if ((add == 0) && (perDay == 0))
                return null;

            string text = "";

            if (add > 0)
                text = add.ToString();

            if (perDay > 0)
            {
                if (add > 0)
                    text += ", ";
                text += $"+{perDay}/д.";
            }

            return text;
        }

        internal static string DecIntegerBy10(int value, bool showPlus = false)
        {
            if (value == 0)
                return "0";

            int val10 = Math.Abs(value) / 10;
            int modval = Math.Abs(value) % 10;

            return (value > 0 ? (showPlus ? "+" : "") : "-") + val10.ToString() + (modval > 0 ? "." + modval.ToString() : "");
        }

        internal static string FormatDecimal100AsInt(int value, bool showPlus = false)
        {
            if (value == 0)
                return "0";

            return (value > 0 ? (showPlus ? "+" : "") : "-") + (Math.Abs(value) / 100).ToString();
        }

        internal static string FormatDecimal100(int value, bool showPlus = false)
        {
            if (value == 0)
                return "0";

            int val100 = Math.Abs(value) / 100;
            int modval = Math.Abs(value) % 100;

            return (value > 0 ? (showPlus ? "+" : "") : "-") + val100.ToString() + (modval > 0 ? "." + modval.ToString() : "");
        }

        internal static string FormatDurability(decimal value)
        {
            return ((int)decimal.Truncate(value)).ToString();
        }

        internal static string FormatPercent(int value, bool showPlus = false)
        {
            return DecIntegerBy10(value) + '%';
        }

        internal static string FormatInteger(int value)
        {
            return (value > 0 ? "+" : "") + value.ToString();
        }

        internal static void Assert(bool condition, string text = "")
        {
            if (!condition)
            {
                Debugger.Break();
                throw new Exception(text);
            }
        }
        
        internal static void DoException(string text)
        {
            Debugger.Break();
            throw new Exception(text);
        }

        internal static Bitmap LoadBitmap(string filename, string folder = "Icons")
        {
            Bitmap bmp = new Bitmap(Program.FolderResources + $@"{folder}\{filename}");
            Debug.Assert(Math.Round(bmp.HorizontalResolution) == DEFAULT_DPI);
            Debug.Assert(Math.Round(bmp.VerticalResolution) == DEFAULT_DPI);

            if ((dpiX != DEFAULT_DPI) || (dpiY != DEFAULT_DPI))
                bmp.SetResolution(dpiX, dpiY);

            return bmp;
        }

        internal static void LackBitmap(Bitmap bmp, Color color)
        {
            if (color != Color.Transparent)
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                byte[] rgbValues = new byte[bytes];
                Marshal.Copy(ptr, rgbValues, 0, bytes);

                for (int counter = 0; counter < rgbValues.Length; counter += 4)
                {
                    if (rgbValues[counter + 3] > 0)
                    {
                        rgbValues[counter + 0] = Convert.ToByte(rgbValues[counter + 0] * color.B / 255);
                        rgbValues[counter + 1] = Convert.ToByte(rgbValues[counter + 1] * color.G / 255);
                        rgbValues[counter + 2] = Convert.ToByte(rgbValues[counter + 2] * color.R / 255);
                    }
                }

                Marshal.Copy(rgbValues, 0, ptr, bytes);
                bmp.UnlockBits(bmpData);
            }
        }
    }
}