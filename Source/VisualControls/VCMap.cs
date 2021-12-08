using System.Drawing;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCMap : VisualControl
    {
        private Bitmap bmp;
        private Point shiftBitmap;
        private Rectangle windowDraw = new Rectangle();

        public VCMap(VisualControl parent, int shiftX, int shiftY, string filenameBitmap) : base(parent, shiftX, shiftY)
        {
            bmp = Program.formMain.LoadBitmap(filenameBitmap, @"Icons\Conq");
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(bmp != null);
            UpdateWindow();

            base.Draw(g);

            g.DrawImage(bmp, Left, Top, windowDraw, GraphicsUnit.Pixel);
        }

        private void UpdateWindow()
        {
            if ((windowDraw.Left != shiftBitmap.X) || (windowDraw.Top != shiftBitmap.Y) || (windowDraw.Width != Width) || (windowDraw.Height !=  Height))
                windowDraw = new Rectangle(shiftBitmap.X, shiftBitmap.Y, Width, Height);
        }

        internal override void MouseMove(Point p, bool leftDown, bool rightDown)
        {
            base.MouseMove(p, leftDown, rightDown);

            //if (dow)
        }
    }
}
