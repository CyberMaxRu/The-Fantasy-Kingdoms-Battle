using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - кнопка c иконкой
    internal class VCIconButton : VCImage
    {
        public VCIconButton(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            UseFilter = true;
            //HighlightUnderMouse = true;
        }

        internal override void Draw(Graphics g)
        {
            //ImageFilter = ImageFilter.Active;
            //if (Visible)
            //    g.DrawImageUnscaled(Program.formMain.bmpBackgroundEntity, Left - 1, Top - 1);

            base.Draw(g);
        }
    }
}
