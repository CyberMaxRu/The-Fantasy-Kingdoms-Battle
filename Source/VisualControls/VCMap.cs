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
        private Point pointRightButtonClicked;
        private Point shiftBitmapRightButtonClicked;

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

        internal override void MouseRightDown(Point p)
        {
            base.MouseRightDown(p);

            pointRightButtonClicked = p;
            shiftBitmapRightButtonClicked = shiftBitmap;
        }

        internal override void RightButtonClick()
        {
            base.RightButtonClick();

            pointRightButtonClicked = new Point(-1, -1);
        }

        internal override void MouseMove(Point p, bool leftDown, bool rightDown)
        {
            base.MouseMove(p, leftDown, rightDown);

            if (rightDown)
            {
                shiftBitmap = new Point(shiftBitmapRightButtonClicked.X + pointRightButtonClicked.X - p.X, shiftBitmapRightButtonClicked.Y + pointRightButtonClicked.Y - p.Y);
                if (shiftBitmap.X < 0)
                    shiftBitmap.X = 0;
                if (shiftBitmap.X > bmp.Width - Width)
                    shiftBitmap.X = bmp.Width - Width;
                if (shiftBitmap.Y < 0)
                    shiftBitmap.Y = 0;
                if (shiftBitmap.Y > bmp.Height - Height)
                    shiftBitmap.Y = bmp.Height - Height;

                UpdateWindow();
            }
        }
    }
}
