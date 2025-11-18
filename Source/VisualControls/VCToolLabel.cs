using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - иконка 16 * 16 с текстом для тулбара
    internal class VCToolLabel : VCLabel
    {
        public VCToolLabel(VisualControl parent, int shiftX, int shiftY, int imageIndex)
            : base(parent, shiftX, shiftY, Program.formMain.FontMedCaption, Color.White, 26, "", Program.formMain.BmpListGui16)
        {
            StringFormat.Alignment = StringAlignment.Far;
            StringFormat.LineAlignment = StringAlignment.Near;
            IsActiveControl = true;

            Image.ImageIndex = imageIndex;
            Width = 112;
            TopMargin = 1;
            RightMargin = 6;

            ShiftImage = new Point(6, 4);
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            Program.formMain.bbToolBarLabel.DrawBorder(g, Left, Top, Width, Height, Color.Transparent);
        }
    }
}
