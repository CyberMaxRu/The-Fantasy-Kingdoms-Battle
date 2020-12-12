using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    internal sealed class GuiUtils
    {

        internal static Label CreateLabel(Control parent, int left, int top)
        {
            Label l = new Label()
            {
                Parent = parent,
                Left = left,
                Top = top
            };

            return l;
        }

        internal static Label CreateLabelParameter(Control parent, int left, int top, int imIndex)
        {
            Label l = new Label()
            {
                Parent = parent,
                Left = left,
                Top = top,
                Width = 80,
                TextAlign = ContentAlignment.MiddleRight,
                ImageList = Program.formMain.ilParameters,
                ImageIndex = imIndex,
                ImageAlign = ContentAlignment.MiddleLeft
            };

            return l;
        }

        internal static Size SizeButtonWithImage(ImageList il)
        {
            return new Size(il.ImageSize.Width + 8, il.ImageSize.Height + 8);
        }

        internal static Size SizePictureBoxWithImage(ImageList il)
        {
            return new Size(il.ImageSize.Width + 2, il.ImageSize.Height + 2);
        }

        internal static int NextLeft(Control c)
        {
            return c.Left + c.Width + FormMain.Config.GridSize;
        }

        internal static int NextTop(Control c)
        {
            return c.Top + c.Height + FormMain.Config.GridSize;
        }

        internal static int NextTopHalf(Control c)
        {
            return c.Top + c.Height + FormMain.Config.GridSizeHalf;
        }

        internal static Image GetImageFromImageList(ImageList imageList, int imageIndex, bool normal)
        {
            return imageList.Images[imageIndex + (normal == true ? 0 : imageList.Images.Count / 2)];
        }

        internal static int GetImageIndexWithGray(ImageList imageList, int imageIndex, bool normal)
        {
            return imageIndex + (normal == true ? 0 : imageList.Images.Count / 2);
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
                ImageList = Program.formMain.ilGui,
                ImageIndex = imageIndex,
                BackColor = Color.Transparent,
                BackgroundImage = Program.formMain.bmpBackgroundButton,
                Size = SizeButtonWithImage(Program.formMain.ilGui)
            };
            //b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.BorderColor = Color.Black;

            return b;
        }
    }
}
