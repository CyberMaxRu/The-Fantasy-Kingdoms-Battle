using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCCustomNotice : VisualControl
    {
        private static List<Bitmap> cacheBackground = new List<Bitmap>();

        protected readonly VCLabel lblCaption;
        protected readonly VCLabel lblText;
        private readonly Bitmap bmpBackground;

        public VCCustomNotice(int width) : base()
        {
            Cell = new VCCellSimple(this, 0, 3);

            lblCaption = new VCLabel(this, Cell.NextLeft(), 4, Program.formMain.fontMedCaptionC, Color.Gray, 16, "");
            lblCaption.ClickOnParent = true;

            lblText = new VCLabel(this, lblCaption.ShiftX, 27, Program.formMain.fontMedCaptionC, Color.Gray, 16, "");
            lblText.ClickOnParent = true;
            Height = 54;
            Width = width;

            bmpBackground = PrepareBackground(width - 52);
        }

        internal VCCellSimple Cell { get; }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            g.DrawImageUnscaled(bmpBackground, Left + 52, Top);
        }

        private Bitmap PrepareBackground(int width)
        {
            Debug.Assert(width >= 48);

            foreach (Bitmap b in cacheBackground)
            {
                if (b.Width == width)
                    return b;
            }

            Bitmap bmp = new Bitmap(width, Height);
            int beginAlpha = 50;
            float stepAlpha = width / beginAlpha;
            // Инициализируем цветом и градиентной прозрачностью
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(Math.Max(0, Convert.ToInt32(beginAlpha - (x / stepAlpha))), Color.SkyBlue));
                }

            return bmp;
        }

        internal void SetNotice(int imageIndex, string caption, string text, Color colorText)
        {
            Cell.ImageIndex = imageIndex;
            lblCaption.Text = caption;
            lblText.Text = text;
            lblText.Color = colorText;

            lblCaption.Width = lblCaption.Font.WidthText(lblCaption.Text);
            lblText.Width = lblText.Font.WidthText(lblText.Text);
        }
    }
}
