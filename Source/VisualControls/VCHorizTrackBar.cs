using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCHorizTrackBar : VisualControl
    {
        private readonly static Bitmap bmpButtonLeft;
        private readonly static Bitmap bmpButtonRight;
        private readonly static Bitmap bmpButtonTrack;
        private readonly static Bitmap bmpTileBackground;
        private readonly static Bitmap bmpTick;
        private readonly static int shiftTick;

        private readonly static BitmapList blButtons;

        private readonly VCImage btnLeft;
        private readonly VCImage btnRight;
        private Bitmap bmpBackground;

        static VCHorizTrackBar()
        {
            bmpTileBackground = Program.formMain.LoadBitmap("ScrollBarHorizBack.png");
            Utils.Assert(bmpTileBackground.Width == 1);

            Bitmap bmpButtons = Program.formMain.LoadBitmap("ScrollBarHorizButtons.png");
            blButtons = new BitmapList(bmpButtons, bmpButtons.Height, true, true);
            bmpButtons.Dispose();

            bmpTick = Program.formMain.LoadBitmap("ScrollBarHorizTick.png");
            shiftTick = bmpTick.Width / 2;
        }

        public VCHorizTrackBar(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            Height = bmpTileBackground.Height;

            btnLeft = new VCImage(this, 0, 0, blButtons, 0);
            btnLeft.HighlightUnderMouse = true;
            btnRight = new VCImage(this, 0, 0, blButtons, 1);
            btnRight.HighlightUnderMouse = true;
        }

        internal int Min { get; set; } = 0;
        internal int Max { get; set; } = 100;
        internal int Position { get; private set; } = 0;
        internal int Frequency { get; set; } = 5;

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            btnRight.ShiftX = Width - btnRight.Width;
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            if ((bmpBackground is null) || (bmpBackground.Width + btnLeft.Width + btnRight.Width != Width))
            {
                bmpBackground?.Dispose();
                bmpBackground = new Bitmap(Width - btnLeft.Width - btnRight.Width, Height);
                Graphics gBody = Graphics.FromImage(bmpBackground);
                for (int i = 0; i < bmpBackground.Width; i++)
                {
                    gBody.DrawImageUnscaled(bmpTileBackground, i, 0);
                }

                gBody.Dispose();
            }

            g.DrawImageUnscaled(bmpBackground, Left + btnLeft.Width, Top);

            if (Frequency > 0)
            {
                float step = (float)bmpBackground.Width / (Frequency + 1);
                for (int i = 1; i <= Frequency; i++)
                {
                    g.DrawImageUnscaled(bmpTick, Left + btnLeft.Width + (int)(i * step) - shiftTick, Top);
                }
            }
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);
        }
    }
}