using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - кнопка c иконкой
    internal class VCIconButton48 : VCImage48
    {
        public VCIconButton48(VisualControl parent, int shiftX, int shiftY, int imageIndex) : base(parent, shiftX, shiftY, imageIndex)
        {
            //UseFilter = true;
            PlaySoundOnClick = true;
            HighlightUnderMouse = true;
            ShowBorder = true;
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
