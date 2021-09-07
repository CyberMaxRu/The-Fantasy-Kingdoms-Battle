using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{

    // Визуальный контрол - иконка
    internal class VCImage : VisualControl
    {
        public VCImage(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
            ImageIndex = imageIndex;
        }

        internal BitmapList BitmapList { get; set; }
        internal int ImageIndex { get; set; }
        internal bool NormalImage { get; set; } = true;
        internal bool ImageIsEnabled { get; set; } = true;
        internal bool HighlightUnderMouse { get; set; } = false;

        internal override void MouseEnter(bool leftButtonDown)
        {
            base.MouseEnter(leftButtonDown);

            Program.formMain.SetNeedRedrawFrame();
            if (PlaySelectSound())
                Program.formMain.PlaySelectButton();
        }

        internal override void MouseLeave()
        {
            base.MouseLeave();

            Program.formMain.SetNeedRedrawFrame();
        }

        protected override bool AllowClick()
        {
            return ImageIsEnabled && base.AllowClick();
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            // Иконка
            if (Visible && (ImageIndex != -1))
            {
                BitmapList.DrawImage(g, ImageIndex, (/*UseFilter*/ ImageIsEnabled) && NormalImage, HighlightUnderMouse && MouseOver && !MouseClicked, Left, Top);
            }
        }

        protected virtual bool PlaySelectSound()
        {
            return true;// ImageIsEnabled && ((UseFilter && MouseOver) || HighlightUnderMouse);
        }
    }
}
