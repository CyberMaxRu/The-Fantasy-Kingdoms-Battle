using System.Drawing;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCMap : VisualControl
    {
        private Point shiftBitmap;
        private Rectangle windowDraw = new Rectangle();
        private Point pointRightButtonClicked;
        private Point shiftBitmapRightButtonClicked;

        public VCMap(VisualControl parent, int shiftX, int shiftY, string filenameBitmap) : base(parent, shiftX, shiftY)
        {
            if (filenameBitmap.Length > 0)
                Bitmap = LoadBitmap(filenameBitmap, @"Icons\Conq");
        }

        internal Bitmap Bitmap { get; }

        internal override void Draw(Graphics g)
        {
            if (Bitmap != null)
            {
                UpdateWindow();
            }

            base.Draw(g);

            if (Bitmap != null)
                g.DrawImage(Bitmap, Left, Top, windowDraw, GraphicsUnit.Pixel);
        }

        private void UpdateWindow()
        {
            if ((windowDraw.Left != shiftBitmap.X) || (windowDraw.Top != shiftBitmap.Y) || (windowDraw.Width != Width) || (windowDraw.Height !=  Height))
                windowDraw = new Rectangle(shiftBitmap.X, shiftBitmap.Y, Width, Height);
        }

        internal override void MouseRightDown(Point p)
        {
            base.MouseRightDown(p);

            if (Bitmap != null)
            {
                pointRightButtonClicked = p;
                shiftBitmapRightButtonClicked = shiftBitmap;
            }
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
                if (shiftBitmap.X > Bitmap.Width - Width)
                    shiftBitmap.X = Bitmap.Width - Width;
                if (shiftBitmap.Y < 0)
                    shiftBitmap.Y = 0;
                if (shiftBitmap.Y > Bitmap.Height - Height)
                    shiftBitmap.Y = Bitmap.Height - Height;

                UpdateWindow();
            }
        }

        internal Point MousePosToCoord(Point p)
        {
            return new Point(shiftBitmap.X + p.X, shiftBitmap.Y + p.Y);
        }
    }
}
