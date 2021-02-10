using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - картинка на основе Bitmap
    internal sealed class VCBitmap : VisualControl
    {
        private readonly Bitmap bmp;
        public VCBitmap(VisualControl parent, int shiftX, int shiftY, Bitmap b) : base(parent, shiftX, shiftY)
        {
            Debug.Assert(b != null);

            bmp = b;
            Width = bmp.Width;
            Height = bmp.Height;
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            g.DrawImageUnscaled(bmp, Left, Top);
        }
    }
}
