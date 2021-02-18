using System;
using System.Text;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - текстовая метка
    internal class VCLabel : VisualControl
    {
        private Brush brush;
        private Color color;
        private RectangleF rectText;

        public VCLabel(VisualControl parent, int shiftX, int shiftY, Font font, Color foreColor, int height, string text) : base(parent, shiftX, shiftY)
        {
            Height = height;
            Text = text;
            Font = font;
            Color = foreColor;

            StringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            };

            LeftMargin = 0;
            // Рисуем выше на два пиксела и увеличиваем высоту, так как у текст сверху пустота, а снизу происходит обрезка,
            // хотя по высоте все вмещается
            TopMargin = -2;
        }

        internal string Text { get; set; }
        internal Font Font { get; set; }
        internal Color Color
        {
            get => Color;
            set { if (color != value) { color = value; brush?.Dispose(); brush = new SolidBrush(color); } }
        }

        internal BitmapList BitmapList { get; set; }
        internal int ImageIndex { get; set; } = -1;
        internal ImageState ImageState { get; set; } = ImageState.Normal;
        protected int LeftMargin { get; set; }
        protected int TopMargin { get; set; }
        internal StringFormat StringFormat { get; set; }

        internal override void Draw(Graphics g)
        {
            if ((BitmapList != null) && (ImageIndex >= 0))
                BitmapList.DrawImage(g, ImageIndex, ImageState, Left, Top);

            base.Draw(g);

            Bitmap bmpSymbol;
            int left = Left;
            byte[] text1251 = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1251), Encoding.UTF8.GetBytes(Text));
            foreach(byte b in text1251)
            {
                bmpSymbol = Program.formMain.fontSmallContur.symbols[b - 32];
                g.DrawImageUnscaled(bmpSymbol, left, Top);
                left += bmpSymbol.Width + 0;
            }
            //g.DrawString(Text, Font, brush, rectText, StringFormat);
        }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            if ((BitmapList != null) && (ImageIndex >= 0))
                LeftMargin = BitmapList.Size + FormMain.Config.GridSize;

            rectText = new RectangleF(Left + LeftMargin, Top + TopMargin, Width, Height + 2);
        }
    }
}