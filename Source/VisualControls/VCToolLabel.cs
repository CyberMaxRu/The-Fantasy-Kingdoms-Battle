using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - иконка 16 * 16 с текстом для тулбара
    internal class VCToolLabel : VCLabel
    {
        public VCToolLabel(VisualControl parent, int shiftX, int shiftY, string text, int imageIndex)
            : base(parent, shiftX, shiftY, Program.formMain.fontMedCaption, Color.White, 26, text, Program.formMain.ilGui16)
        {
            StringFormat.Alignment = StringAlignment.Near;
            StringFormat.LineAlignment = StringAlignment.Near;
            IsActiveControl = true;

            Image.ImageIndex = imageIndex;
            Width = 80;
            TopMargin = 2;

            ShiftImage = new Point(6, 4);
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            Program.formMain.bbToolBarLabel.DrawBorder(g, Left, Top, Width, Height, Color.Transparent);
        }
    }
}
