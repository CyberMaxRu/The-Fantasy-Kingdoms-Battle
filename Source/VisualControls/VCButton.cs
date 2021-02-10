namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - кнопка c иконкой
    internal class VCButton : VCImage
    {
        public VCButton(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            ShowBorder = true;
            ShiftImage = 2;
        }

        internal override void MouseEnter()
        {
            base.MouseEnter();

            ImageState = ImageState.Over;
            Program.formMain.NeedRedrawFrame();
        }

        internal override void MouseLeave()
        {
            base.MouseLeave();

            ImageState = ImageState.Normal;
            Program.formMain.NeedRedrawFrame();
        }
    }
}
