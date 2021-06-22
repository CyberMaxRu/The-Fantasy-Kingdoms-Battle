using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - текстовая метка, использующая оригинальный шрифт из Majesty 2

    internal class VCLabelM2 : VisualControl
    {
        private RectangleF rectText;
        private Bitmap bmpPreparedText;
        private string preparedText;
        private Color preparedColor;

        public VCLabelM2(VisualControl parent, int shiftX, int shiftY, M2Font font, Color foreColor, int height, string text) : base(parent, shiftX, shiftY)
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
            TopMargin = 0;
        }

        internal string Text { get; set; }
        internal M2Font Font { get; set; }
        internal Color Color { get; set; }

        internal BitmapList BitmapList { get; set; }
        internal int ImageIndex { get; set; } = -1;
        internal bool ImageIsEnabled { get; set; } = true;
        internal bool ImageIsOver { get; set; } = false;
        protected int LeftMargin { get; set; }
        protected int RightMargin { get; set; }
        internal int TopMargin { get; set; }
        internal StringFormat StringFormat { get; set; }
        internal Point ShiftImage { get; set; } = new Point(0, 0);
        internal bool TruncLongText { get; set; } = false;

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Width > 0);

            if (Visible || ManualDraw)
            {
                if ((BitmapList != null) && (ImageIndex >= 0))
                {
                    BitmapList.DrawImage(g, ImageIndex, ImageIsEnabled, ImageIsOver, Left + ShiftImage.X, Top + ShiftImage.Y);
                }

                if (Text.Length > 0)
                {
                    if ((preparedText != Text) || (preparedColor != Color))
                    {
                        if (TruncLongText && (Font.WidthText(Text) > Width))
                        {
                            int restSymbols = Text.Length - 1;
                            string truncedText;
                            while (restSymbols > 0)
                            {
                                truncedText = Text.Substring(0, restSymbols) + "...";
                                if (Font.WidthText(truncedText) <= Width)
                                {
                                    preparedText = truncedText;
                                    break;
                                }

                                restSymbols--;
                            }

                            Debug.Assert(restSymbols > 0);
                        }
                        else
                            preparedText = Text;

                        bmpPreparedText?.Dispose();
                        bmpPreparedText = Font.GetBitmap(preparedText, Color);
                        preparedColor = Color;
                    }
                }
            }

            base.Draw(g);

            if (Visible || ManualDraw)
            {
                if (Text.Length > 0)
                {
                    int x;
                    int y;
                    switch (StringFormat.Alignment)
                    {
                        case StringAlignment.Near:
                            x = Left;
                            break;
                        case StringAlignment.Center:
                            x = Left + ((Width - bmpPreparedText.Width) / 2);
                            break;
                        default:
                            x = Left + Width - bmpPreparedText.Width - LeftMargin - RightMargin;
                            break;
                    }

                    Debug.Assert(x >= Left);

                    switch (StringFormat.LineAlignment)
                    {
                        case StringAlignment.Near:
                            y = Top;
                            break;
                        case StringAlignment.Center:
                            y = Top + ((Height - bmpPreparedText.Height) / 2);
                            break;
                        default:
                            y = Top + Height - bmpPreparedText.Height;
                            break;
                    }
                    //Debug.Assert(y >= Top);

                    Debug.Assert(bmpPreparedText.Width + LeftMargin + RightMargin <= Width, $"Текст {preparedText} занимает {bmpPreparedText.Width} пикселей (LeftMargin {LeftMargin}, RightMargin {RightMargin}), не вмещаясь в {Width}.");

                    g.DrawImageUnscaled(bmpPreparedText, x + LeftMargin, y + TopMargin);
                }
            }
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
