using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    public sealed class GuiUtils
    {
        internal static int NextTop(Control c)
        {
            return c.Top + c.Height + FormMain.Config.GridSize;
        }

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

        internal static Bitmap MakeBackgroundWithBorder(Size size, Color borderColor)
        {
            Bitmap bmp = MakeBackground(size);

            Graphics g = Graphics.FromImage(bmp);
            Pen penBorder = new Pen(borderColor);
            g.DrawRectangle(penBorder, 0, 0, bmp.Width - 1, bmp.Height - 1);

            penBorder.Dispose();
            g.Dispose();

            return bmp;
        }

        internal static PictureBox BorderSelected(Control parent, Size size)
        {
            PictureBox pbox = new PictureBox()
            {
                Width = size.Width + 3 + 3,
                Height = size.Height + 2 + 5,
                Parent = parent
            };
            
            return pbox;
        }

        internal static Label CreateLabel(Control parent, int left, int top, int width, string text)
        {
            return new Label()
            {
                Parent = parent,
                Left = left,
                Top = top,
                AutoSize = false,
                Width = width,
                Text = text
            };
        }

        internal static Button CreateButtonWithIcon(Control parent, int left, int top, int imageIndex)
        {
            Button b = new Button()
            {
                Parent = parent,
                Left = left,
                Top = top,
                FlatStyle = FlatStyle.Flat,
                //ImageList = Program.formMain.ilGui,
                ImageIndex = imageIndex,
                BackColor = Color.Transparent
                //Size = SizeButtonWithImage(Program.formMain.ilGui)
            };
            //b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.BorderColor = Color.Black;

            return b;
        }

        public static void ShowError(string text)
        {
            MessageBox.Show(text, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal static Bitmap PrepareAvatar(string filename)
        {
            try
            {
                Bitmap bmpRaw = new Bitmap(filename);
                Bitmap newAvatar = new Bitmap(Program.formMain.ilPlayerAvatarsBig.Size, Program.formMain.ilPlayerAvatarsBig.Size);

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
                gAvatar.Dispose();

                return newAvatar;
            }
            catch (Exception e)
            {
                ShowError("Ошибка при загрузке пользовательского аватара: " + e.Message);
                return null;
            }
        }

        internal static Bitmap ApplyDisappearance(Image i, int curDisappearance, int MaxValue)
        {
            Debug.Assert(curDisappearance <= MaxValue);

            double percent = (double)curDisappearance / MaxValue;

            Bitmap b = new Bitmap(i);
            for (int y = 0; y < b.Height; y++)
                for (int x = 0; x < b.Width; x++)
                {
                    Color pixel = b.GetPixel(x, y);
                    b.SetPixel(x, y, Color.FromArgb((int)(pixel.A * percent), pixel));
                }

            return b;
        }
    }
}
