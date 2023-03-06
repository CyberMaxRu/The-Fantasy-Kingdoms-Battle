using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс прогрессбара
    internal sealed class VCProgressBar : VCBitmapBand
    {
        private VCProgressBarBack ppbBack;
        private VCProgressBarFore ppfFore;
        private VCLabel lblText;

        public VCProgressBar(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, 17)
        {
            lblText = new VCLabel(this, 0, 2, Program.formMain.fontSmallC, Color.White, 16, "");
            lblText.Visible = false;
            lblText.ManualDraw = true;
            lblText.StringFormat.Alignment = StringAlignment.Center;
            lblText.StringFormat.LineAlignment = StringAlignment.Near;
        }

        internal int Max { get; set; }
        internal int Position { get; set; }
        internal string Text { get; set; } = "";
        protected override Bitmap GetBitmap() => Program.formMain.bmpBandProgressBar;
        internal Color ColorProgress { get; set; } = Color.Transparent;

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            lblText.Width = Width;
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            Utils.Assert(Position >= 0);
            Utils.Assert(Position <= Max);

            if ((ppbBack is null) || (ppbBack.Width + 18 != Width))
            {
                ppbBack?.Dispose();
                ppbBack = new VCProgressBarBack(this, 10, 5);
                ppbBack.Width = Width - 18;
                ppbBack.Visible = false;
                ArrangeControl(ppbBack);
            }

            ppbBack.Draw(g);

            if (Max > 0)
            {
                int length = (ppbBack.Width - 1) * Position / Max;

                // Определяем длину прогресса
                if (length > 0)
                {
                    if ((ppfFore is null) || (ppfFore.Width != length) || (ppfFore.Color != ColorProgress))
                    {
                        ppfFore?.Dispose();

                        ppfFore = new VCProgressBarFore(this, 11, 5);
                        ppfFore.TruncateLeft = true;
                        ppfFore.Width = length;
                        ppfFore.Color = ColorProgress;
                        ppfFore.Visible = false;
                        ArrangeControl(ppfFore);
                    }

                    ppfFore.Draw(g);
                }
            }
        }

        internal override void PaintForeground(Graphics g)
        {
            base.PaintForeground(g);

            if (Text.Length > 0)
            {
                lblText.Text = Text;
                lblText.Draw(g);
            }
        }
    }

    internal sealed class VCProgressBarBack : VCBitmapBand
    {
        public VCProgressBarBack(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, 7)
        {
        }

        protected override Bitmap GetBitmap() => Program.formMain.bmpBandProgressBarBack;
    }

    internal sealed class VCProgressBarFore : VCBitmapBand
    {
        public VCProgressBarFore(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, 8)
        {
        }

        protected override Bitmap GetBitmap() => Program.formMain.bmpBandProgressBarFore;
    }
}
