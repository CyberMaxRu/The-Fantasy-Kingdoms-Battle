using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{

    // Визуальный контрол - иконка
    internal class VCImage : VisualControl
    {
        private BitmapList bitmapList;

        public VCImage(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
            ImageIndex = imageIndex;

            Width = BitmapList.Size.Width;
            Height = BitmapList.Size.Height;
        }

        internal BitmapList BitmapList
        {
            get => bitmapList;
            set
            {
                bitmapList = value;
                if ((Width != bitmapList.Size.Width) || (Height != bitmapList.Size.Height))
                {
                    Width = bitmapList.Size.Width;
                    Height = bitmapList.Size.Height;
                }
            }
        }
        internal int ImageIndex { get; set; }
        internal bool ImageIsEnabled { get; set; } = true;
        internal bool HighlightUnderMouse { get; set; } = false;

        internal override void MouseEnter(bool leftButtonDown)
        {
            base.MouseEnter(leftButtonDown);

            if (PlaySelectSound())
                Program.formMain.PlaySelectButton();
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            // Иконка
            if ((Visible || ManualDraw) && (ImageIndex != -1))
            {
                BitmapList.DrawImage(g, ImageIndex, /*UseFilter*/ ImageIsEnabled, HighlightUnderMouse && MouseOver && !MouseClicked, Left, Top, Opacity);
            }
        }

        protected virtual bool PlaySelectSound()
        {
            return true;// ImageIsEnabled && ((UseFilter && MouseOver) || HighlightUnderMouse);
        }

        protected override void ValidateRectangle()
        {
            base.ValidateRectangle();

        }
    }
}
