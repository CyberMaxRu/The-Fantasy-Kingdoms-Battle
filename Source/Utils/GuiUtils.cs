﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Fantasy_Kingdoms_Battle
{
    public sealed class GuiUtils
    {
        internal static void DrawBand(Graphics g, Rectangle r, Brush brushFore, Brush brushBack, int currentValue, int MaxValue)
        {
            Debug.Assert(currentValue <= MaxValue);
            Debug.Assert(currentValue >= 0);

            int widthMain = (int)Math.Round(currentValue / 1.00 / MaxValue * r.Width);
            int widthNone = r.Width - widthMain;

            g.FillRectangle(brushFore, r.Left, r.Top, widthMain, r.Height);
            if (widthNone > 0)
                g.FillRectangle(brushBack, r.Left + widthMain, r.Top, widthNone, r.Height);
        }

        internal static Bitmap MakeCustomBackground(Bitmap texture, VisualControl control)
        {
            Debug.Assert(control.Width > FormMain.Config.GridSize);
            Debug.Assert(control.Height > FormMain.Config.GridSize);

            Bitmap bmpBackground = new Bitmap(control.Width, control.Height);
            Graphics g = Graphics.FromImage(bmpBackground);
            g.CompositingMode = CompositingMode.SourceCopy;

            int repX = control.Width / texture.Width + 1;
            int repY = control.Height / texture.Height + 1;

            for (int y = 0; y < repY; y++)
                for (int x = 0; x < repX; x++)
                    g.DrawImageUnscaled(texture, x * texture.Width, y * texture.Height);

            g.Dispose();

            return bmpBackground;
        }

        internal static Bitmap MakeBackground(Size size)
        {
            Debug.Assert(size.Width > FormMain.Config.GridSize);
            Debug.Assert(size.Height > FormMain.Config.GridSize);

            Bitmap bmpBackground = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bmpBackground);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            int repX = size.Width / Program.formMain.bmpForBackground.Width + 1;
            int repY = size.Height / Program.formMain.bmpForBackground.Height + 1;

            for (int y = 0; y < repY; y++)
                for (int x = 0; x < repX; x++)
                    g.DrawImageUnscaled(Program.formMain.bmpForBackground, x * Program.formMain.bmpForBackground.Width, y * Program.formMain.bmpForBackground.Height);

            g.Dispose();

            return bmpBackground;
        }

        public static void ShowError(string text)
        {
            MessageBox.Show(text, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowException(Exception e)
        {
            MessageBox.Show(e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal static Bitmap PrepareAvatar(string filename)
        {
            try
            {
                Bitmap bmpRaw = new Bitmap(filename);
                if (bmpRaw.Width != bmpRaw.Height)
                    throw new Exception($"Аватар должен быть квадратный (текущий размер {bmpRaw.Size}).");

                Bitmap newAvatar = new Bitmap(Program.formMain.BmpListObjects128.Size.Width, Program.formMain.BmpListObjects128.Size.Height);

                Graphics gAvatar = Graphics.FromImage(newAvatar);
                gAvatar.InterpolationMode = InterpolationMode.High;
                // У оригинальных картинок размер 126 * 126
                gAvatar.DrawImage(bmpRaw, new Rectangle(1, 1, newAvatar.Width - 2, newAvatar.Height - 2), new Rectangle(0, 0, bmpRaw.Width, bmpRaw.Height), GraphicsUnit.Pixel);

                // Применяем маску для аватара
                for (int y = 0; y < newAvatar.Height; y++)
                    for (int x = 0; x < newAvatar.Width; x++)
                    {                        
                        newAvatar.SetPixel(x, y, Color.FromArgb(Program.formMain.bmpMaskBig.GetPixel(x, y).A, newAvatar.GetPixel(x, y)));
                    }

                //
                bmpRaw.Dispose();
                gAvatar.Dispose();

                return newAvatar;
            }
            catch (Exception e)
            {
                ShowError("Ошибка при загрузке пользовательского аватара: " + Environment.NewLine + e.Message);
                return null;
            }
        }

        // Изменение прозрачности
        internal static Bitmap ApplyDisappearance(Image i, int curDisappearance, int MaxValue)
        {
            Debug.Assert(curDisappearance <= MaxValue);

            double percent = (double)curDisappearance / MaxValue;
            Bitmap b = new Bitmap(i);

            Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);
            BitmapData bmpData = b.LockBits(rect, ImageLockMode.ReadWrite, b.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * b.Height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (int counter = 0; counter < rgbValues.Length; counter += 4)
            {
                rgbValues[counter + 3] = Convert.ToByte(rgbValues[counter + 3] * percent);
            }

            Marshal.Copy(rgbValues, 0, ptr, bytes);
            b.UnlockBits(bmpData);

            /*for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    Color pixel = b.GetPixel(x, y);
                    b.SetPixel(x, y, Color.FromArgb((int)(pixel.A * percent), pixel));
                }
*/
            return b;
        }
    }
}
