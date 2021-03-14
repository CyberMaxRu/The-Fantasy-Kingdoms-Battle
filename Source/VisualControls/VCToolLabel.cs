using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - иконка 16 * 16 с текстом для тулбара
    internal sealed class VCToolLabel : VCLabel
    {
        private Bitmap bmpBackround;

        public VCToolLabel(VisualControl parent, int shiftX, int shiftY, string text, int imageIndex)
            : base(parent, shiftX, shiftY, FormMain.Config.FontToolbar, Color.White, 26, text)
        {
            StringFormat.Alignment = StringAlignment.Near;
            StringFormat.LineAlignment = StringAlignment.Near;

            BitmapList = Program.formMain.ilGui16;
            ImageIndex = imageIndex;
            Width = 80;
            TopMargin = 3;

            ShiftImage = new Point(5, 4);
        }

        protected override void ValidateRectangle()
        {
            base.ValidateRectangle();

            if ((Width > 0) && (Height > 0) && ((bmpBackround == null) || (bmpBackround.Width != Width) || (bmpBackround.Height != Height)))
            {
                bmpBackround?.Dispose();
                bmpBackround = Program.formMain.bbToolBarLabel.DrawBorder(Width, Height);
            }
        }

        internal override void Draw(Graphics g)
        {
            g.DrawImageUnscaled(bmpBackround, Left, Top);

            base.Draw(g);
        }

    }
}
