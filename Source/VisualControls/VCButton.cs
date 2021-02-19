using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - кнопка c иконкой
    internal class VCButton : VCImage
    {
        public VCButton(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            UseFilter = true;
            //HighlightUnderMouse = true;
        }

        internal override void Draw(Graphics g)
        {
            ImageFilter = ImageFilter.Active;

            base.Draw(g);
        }
    }
}
