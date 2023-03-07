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
        private Bitmap bmpBackground;

        public VCCustomNotice(int width) : base()
        {
            CellOwner = new VCCellSimple(this, 0, 0);
            CellEntity = new VCCellSimple(this, CellOwner.NextLeft(), 0);

            lblCaption = new VCLabel(this, CellEntity.NextLeft() + 2, 0, Program.formMain.fontParagraphC, Color.White, 16, "");

            lblText = new VCLabel(this, lblCaption.ShiftX, 24, Program.formMain.fontParagraphC, Color.White, 16, "");
            Height = CellEntity.Height;
            Width = width;
        }

        internal VCCellSimple CellOwner { get; }
        internal VCCellSimple CellEntity { get; }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            g.DrawImageUnscaled(bmpBackground, CellEntity.Left + CellEntity.Width + FormMain.Config.GridSize, Top);
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
            int beginAlpha = 140;
            int endAlpha = 50;
            float stepAlpha = (float)width / (beginAlpha - endAlpha);
            // Инициализируем цветом и градиентной прозрачностью
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(Math.Max(0, Convert.ToInt32(beginAlpha - (x / stepAlpha))), Color.SkyBlue));
                }

            cacheBackground.Add(bmp);

            return bmp;
        }

        internal void SetNotice(int imageIndexOwner, int imageIndexEntity, string caption, string text, Color colorText)
        {
            CellOwner.ImageIndex = imageIndexOwner;
            CellEntity.ImageIndex = imageIndexEntity;
            lblCaption.Text = caption;
            lblText.Text = text;
            lblText.Color = colorText;

            if (CellOwner.ImageIndex == -1)
            {
                int shift = CellEntity.ShiftX - CellOwner.ShiftX;
                CellOwner.Visible = false;
                CellEntity.ShiftX -= shift;
                lblCaption.ShiftX -= shift;
                lblText.ShiftX -= shift;
            }

            bmpBackground = PrepareBackground(Width - lblCaption.ShiftX + 2);

            lblCaption.Width = lblCaption.Font.WidthText(lblCaption.Text);
            lblText.Width = lblText.Font.WidthText(lblText.Text);
        }
    }
}
