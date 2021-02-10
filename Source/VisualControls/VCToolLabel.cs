using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - иконка 16 * 16 с текстом для тулбара
    internal sealed class VCToolLabel : VCLabel
    {
        public VCToolLabel(VisualControl parent, int shiftX, int shiftY, string text, int imageIndex)
            : base(parent, shiftX, shiftY, FormMain.Config.FontToolbar, Color.White, 20, text)
        {
            StringFormat.Alignment = StringAlignment.Near;
            StringFormat.LineAlignment = StringAlignment.Near;

            BitmapList = Program.formMain.ilGui16;
            ImageIndex = imageIndex;
            Width = 80;
            TopMargin = 0;
        }
    }
}
