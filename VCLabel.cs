using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс метки
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

        internal ImageList ImageList { get; set; }
        internal int ImageIndex { get; set; } = -1;
        protected int LeftMargin { get; set; }
        protected int TopMargin { get; set; }

        internal StringFormat StringFormat { get; set; }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            if ((ImageList != null) && (ImageIndex > 0))
                LeftMargin = ImageList.ImageSize.Width + FormMain.Config.GridSize;

            rectText = new RectangleF(Left + LeftMargin, Top + TopMargin, Width, Height + 2);
        }

        internal override void Draw(Graphics g)
        {
            if ((ImageList != null) && (ImageIndex > 0))
                g.DrawImageUnscaled(GuiUtils.GetImageFromImageList(ImageList, ImageIndex, true), Left, Top);

            base.Draw(g);

            g.DrawString(Text, Font, brush, rectText, StringFormat);
        }
    }
}
