namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - кнопка c иконкой
    internal class VCButton : VCImage
    {
        public VCButton(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            ShowBorder = true;
            ShiftImage = 2;
            HighlightUnderMouse = true;
        }
    }
}
