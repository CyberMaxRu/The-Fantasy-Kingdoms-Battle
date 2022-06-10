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
        private VCProgressBarFore ppfForePotential;
        private VCProgressBarFore ppfFore;
        private VCLabel lblText;

        public VCProgressBar(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            lblText = new VCLabel(this, 0, 4, Program.formMain.fontSmallC, Color.White, 16, "");
            lblText.Visible = false;
            lblText.ManualDraw = true;
            lblText.StringFormat.Alignment = StringAlignment.Center;
            lblText.StringFormat.LineAlignment = StringAlignment.Near;

            Height = GetBitmap().Height;
        }

        //internal bool Enabled { get => enabled; set { enabled = value; PlaySoundOnEnter = enabled; Program.formMain.NeedRedrawFrame(); } }
        internal int Max { get; set; }
        internal int Position { get; set; }
        internal int PositionPotential { get; set; }
        internal Color Color { get; set; }
        internal bool ShowProgress { get; set; } = true;
        protected override int WidthCap() => 17;
        protected override Bitmap GetBitmap() => Program.formMain.bmpBandProgressBar;

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            lblText.Width = Width;
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            Utils.Assert(Max > 0);
            Utils.Assert(Position >= 0);
            Utils.Assert(Position <= Max);

            if (PositionPotential > 0)
            {
                Utils.Assert(PositionPotential >= Position);
                Utils.Assert(PositionPotential <= Max);
            }

            if ((ppbBack is null) || (ppbBack.Width + 18 != Width))
            {
                ppbBack?.Dispose();
                ppbBack = new VCProgressBarBack(this, 10, 5);
                ppbBack.Width = Width - 18;
                ppbBack.Visible = false;
                ArrangeControl(ppbBack);
            }

            ppbBack.Draw(g);

            int length = (ppbBack.Width - 1) * Position / Max;

            // Определяем длину потенциального прогресса
            if ((PositionPotential > 0) && (PositionPotential > Position))
            {
                int lengthPotential = (ppbBack.Width - 1) * PositionPotential / Max;
                if (lengthPotential > length)
                {
                    Color colorPotential = Color.FromArgb(Color.A, Convert.ToByte(Color.R * 0.5), Convert.ToByte(Color.G * 0.5), Convert.ToByte(Color.B * 0.5));

                    if ((ppfForePotential is null) || (ppfForePotential.Width != lengthPotential) || (ppfForePotential.Color != colorPotential))
                    {
                        ppfForePotential?.Dispose();

                        ppfForePotential = new VCProgressBarFore(this, 11, 5);
                        ppfForePotential.TruncateLeft = true;
                        ppfForePotential.Width = lengthPotential;
                        ppfForePotential.Color = colorPotential;
                        ppfForePotential.Visible = false;
                        ArrangeControl(ppfForePotential);
                    }

                    ppfForePotential.Draw(g);
                }
            }

            // Определяем длину прогресса
            if (length > 0)
            {
                if ((ppfFore is null) || (ppfFore.Width != length) || (ppfFore.Color != Color))
                {
                    ppfFore?.Dispose();

                    ppfFore = new VCProgressBarFore(this, 11, 5);
                    ppfFore.TruncateLeft = true;
                    ppfFore.Width = length;
                    ppfFore.Color = Color;
                    ppfFore.Visible = false;
                    ArrangeControl(ppfFore);
                }

                ppfFore.Draw(g);
            }
        }

        internal override void PaintForeground(Graphics g)
        {
            base.PaintForeground(g);

            if (ShowProgress)
            {
                lblText.Text = Position.ToString() + "/" + Max.ToString();
                lblText.Draw(g);
            }
        }
    }

    internal sealed class VCProgressBarBack : VCBitmapBand
    {
        public VCProgressBarBack(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            Height = GetBitmap().Height;
        }

        protected override int WidthCap() => 7;
        protected override Bitmap GetBitmap() => Program.formMain.bmpBandProgressBarBack;
    }

    internal sealed class VCProgressBarFore : VCBitmapBand
    {
        private Color color;

        public VCProgressBarFore(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            Height = GetBitmap().Height;
        }

        internal Color Color { get; set; }
        protected override int WidthCap() => 8;
        protected override Bitmap GetBitmap() => Program.formMain.bmpBandProgressBarFore;

        internal override void Draw(Graphics g)
        {
            if (Color != color)
            {
                bmpForDraw?.Dispose();
                bmpForDraw = PrepareBand(GetBitmap());
                Utils.LackBitmap(bmpForDraw, Color);

                color = Color;
            }

            base.Draw(g);
        }
    }
}
