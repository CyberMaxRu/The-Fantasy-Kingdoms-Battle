using System;
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
        private Point pointMouse;

        public VCMap(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
        }

        internal DescriptorMap Map { get; set; }

        internal override void Draw(Graphics g)
        {
            if (Map?.Bitmap != null)
            {
                UpdateWindow();
            }

            base.Draw(g);

            if (Map?.Bitmap != null)
                g.DrawImage(Map.Bitmap, Left, Top, windowDraw, GraphicsUnit.Pixel);
        }

        private void UpdateWindow()
        {
            if ((windowDraw.Left != shiftBitmap.X) || (windowDraw.Top != shiftBitmap.Y) || (windowDraw.Width != Width) || (windowDraw.Height !=  Height))
                windowDraw = new Rectangle(shiftBitmap.X, shiftBitmap.Y, Width, Height);
        }

        internal override void MouseRightDown(Point p)
        {
            base.MouseRightDown(p);

            if (Map.Bitmap != null)
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
                if (shiftBitmap.X > Map.Bitmap.Width - Width)
                    shiftBitmap.X = Map.Bitmap.Width - Width;
                if (shiftBitmap.Y < 0)
                    shiftBitmap.Y = 0;
                if (shiftBitmap.Y > Map.Bitmap.Height - Height)
                    shiftBitmap.Y = Map.Bitmap.Height - Height;

                UpdateWindow();
            }

            pointMouse = new Point(shiftBitmap.X + p.X, shiftBitmap.Y + p.Y);

            PanelHint.HideHint();
            PanelHint.SetControl(this);
        }

        internal Point MousePosToCoord(Point p)
        {
            return new Point(shiftBitmap.X + p.X, shiftBitmap.Y + p.Y);
        }

        internal override bool PrepareHint()
        {
            if (Map != null)
            {
                switch (Map.PointsMap[pointMouse.Y, pointMouse.X].TypePoint)
                {
                    case TypePointMap.Undefined:
                        PanelHint.AddSimpleHint("Область не определена");
                        break;
                    case TypePointMap.Border:
                        PanelHint.AddSimpleHint("Граница");
                        break;
                    case TypePointMap.Region:
                        PanelHint.AddStep2Header(Map.PointsMap[pointMouse.Y, pointMouse.X].Region.Name);
                        PanelHint.AddStep3Type("Область");
                        break;
                    default:
                        throw new Exception("Неизвестный тип точки");
                }

                return true;
            }
            else
                return false;
        }
    }
}
