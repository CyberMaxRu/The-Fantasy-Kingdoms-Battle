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
        private readonly static int shiftTracker;

        private readonly static BitmapList blButtons;
        private readonly static BitmapList blTracker;

        private readonly VCImage btnLeft;
        private readonly VCImage btnRight;
        private readonly VCImage btnTracker;
        private int widthTrackband;
        private Bitmap bmpBackground;

        static VCHorizTrackBar()
        {
            bmpTileBackground = Program.formMain.LoadBitmap("ScrollBarHorizBack.png");
            Utils.Assert(bmpTileBackground.Width == 1);

            Bitmap bmpButtons = Program.formMain.LoadBitmap("ScrollBarHorizButtons.png");
            blButtons = new BitmapList(bmpButtons, new Size(bmpButtons.Height, bmpButtons.Height), true, true);

            Bitmap bmpTracker = Program.formMain.LoadBitmap("ScrollBarHorizTracker.png");
            blTracker = new BitmapList(bmpTracker, new Size(bmpTracker.Width, bmpTracker.Height), true, true);
            shiftTracker = blTracker.Size.Width / 2;

            bmpTick = Program.formMain.LoadBitmap("ScrollBarHorizTick.png");
            shiftTick = bmpTick.Width / 2;
        }

        public VCHorizTrackBar(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            Height = bmpTileBackground.Height;

            btnLeft = new VCImage(this, 0, 0, blButtons, 0);
            btnLeft.HighlightUnderMouse = true;
            btnLeft.Click += BtnLeft_Click;
            btnRight = new VCImage(this, 0, 0, blButtons, 1);
            btnRight.HighlightUnderMouse = true;
            btnRight.Click += BtnRight_Click;

            btnTracker = new VCImage(this, 0, 0, blTracker, 0);
        }

        private void BtnRight_Click(object sender, EventArgs e)
        {
            if (Position < Max)
            {
                Position++;
                Program.formMain.SetNeedRedrawFrame();
            }
        }

        private void BtnLeft_Click(object sender, EventArgs e)
        {
            if (Position > Min)
            {
                Position--;
                Program.formMain.SetNeedRedrawFrame();
            }
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

                widthTrackband = Width - btnLeft.Width - btnRight.Width - shiftTracker - shiftTracker + 2;
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
            Utils.Assert(Position >= Min);
            Utils.Assert(Position <= Max);

            btnTracker.ShiftX = btnLeft.Width - 1 + (Position * widthTrackband / 100) - shiftTracker;
            ArrangeControl(btnTracker);

            base.Draw(g);
        }

        internal override void DoClick()
        {
            base.DoClick();

            Point mp = Program.formMain.MousePosToControl(this);
            int posAtTrackband = mp.X - btnLeft.Width;
            if ((posAtTrackband < 0) || (posAtTrackband > widthTrackband))
                return;

            Position = posAtTrackband * 100 / widthTrackband;

            Program.formMain.SetNeedRedrawFrame();
        }
    }
}