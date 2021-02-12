namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - кнопка c иконкой
    internal class VCButton : VCImage
    {
        public VCButton(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            ShowBorder = true;
            ShiftImageX = 2;
            ShiftImageY = 2;
            HighlightUnderMouse = true;
        }
    }
}
