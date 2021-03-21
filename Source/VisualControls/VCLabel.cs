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
        internal bool ImageIsEnabled { get; set; } = true;
        internal bool ImageIsOver { get; set; } = false;
        protected int LeftMargin { get; set; }
        protected int TopMargin { get; set; }
        internal StringFormat StringFormat { get; set; }
        internal Point ShiftImage { get; set; } = new Point(0, 0);

        internal override void Draw(Graphics g)
        {
            if ((BitmapList != null) && (ImageIndex >= 0))
                BitmapList.DrawImage(g, ImageIndex, ImageIsEnabled, ImageIsOver, Left + ShiftImage.X, Top + ShiftImage.Y);

            base.Draw(g);

            g.DrawString(Text, Font, brush, rectText, StringFormat);
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