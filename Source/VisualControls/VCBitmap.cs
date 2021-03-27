using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - картинка на основе Bitmap
    internal sealed class VCBitmap : VisualControl
    {
        private Bitmap bmp;

        public VCBitmap(VisualControl parent, int shiftX, int shiftY, Bitmap b) : base(parent, shiftX, shiftY)
        {
            Bitmap = b;
        }

        public VCBitmap(VisualLayer vl) : base(vl)
        {
        }

        internal Bitmap Bitmap
        {
            get => bmp;
            set
            {
                bmp = value;
                AdjustSize();
            }
        }

        private void AdjustSize()
        {
            if (bmp != null)
            {

                Width = bmp.Width;
                Height = bmp.Height;
            }
            else
            {
                Width = 0;
                Height = 0;
            }
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(bmp != null);

            g.DrawImageUnscaled(bmp, Left, Top);

            base.Draw(g);
        }
    }
}
