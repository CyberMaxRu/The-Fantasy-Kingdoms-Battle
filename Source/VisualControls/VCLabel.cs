using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - текстовая метка, использующая оригинальный шрифт из Majesty 2

    internal class VCLabel : VisualControl
    {
        private Bitmap bmpPreparedText;
        private string preparedText;
        private Color preparedColor;

        public VCLabel(VisualControl parent, int shiftX, int shiftY, M2Font font, Color foreColor, int height, string text, BitmapList bitmapList = null) : base(parent, shiftX, shiftY)
        {
            Height = height;
            Text = text;
            Font = font;
            Color = foreColor;
            IsActiveControl = false;

            StringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            };

            LeftMargin = 0;
            RightMargin = 0;
            // Рисуем выше на два пиксела и увеличиваем высоту, так как у текст сверху пустота, а снизу происходит обрезка,
            // хотя по высоте все вмещается
            TopMargin = 0;

            if (bitmapList != null)
            {
                Image = new VCImage(this, 0, 0, bitmapList, -1);
                Image.IsActiveControl = false;
            }
        }

        internal string Text { get; set; }
        internal M2Font Font { get; set; }
        internal Color Color { get; set; }

        internal VCImage Image { get; }
        internal Point ShiftImage { get; set; } = new Point(0, 0);

        internal int LeftMargin { get; set; }
        internal int RightMargin { get; set; }
        internal int TopMargin { get; set; }
        internal StringFormat StringFormat { get; set; }
        internal bool TruncLongText { get; set; } = false;
        internal bool TextTrunced { get; private set; } = false;

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Text != null);
            if (Width <= 0)
                return;
            Debug.Assert(Width > 0);

            if (Image != null)
            {
                Image.ShiftX = ShiftImage.X;
                Image.ShiftY = ShiftImage.Y;
                ArrangeControl(Image);
            }

            if (Visible || ManualDraw)
            {
                TextTrunced = false;

                if (Text.Length > 0)
                {
                    if ((preparedText != Text) || (preparedColor != Color))
                    {
                        int needWidth = Width - LeftMargin - RightMargin;
                        if (TruncLongText && (Font.WidthText(Text) > needWidth))
                        {
                            TextTrunced = true;

                            int restSymbols = Text.Length - 1;
                            string truncedText;
                            while (restSymbols > 0)
                            {
                                truncedText = Text.Substring(0, restSymbols) + "...";
                                if (Font.WidthText(truncedText) <= needWidth)
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
                            x = Left + LeftMargin;
                            break;
                        case StringAlignment.Center:
                            x = Left + LeftMargin + ((Width - LeftMargin - RightMargin - bmpPreparedText.Width) / 2);
                            break;
                        default:
                            x = Left + Width - bmpPreparedText.Width - RightMargin;
                            break;
                    }

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
                    Debug.Assert(x >= Left);

                    DrawImage(g, bmpPreparedText, x, y + TopMargin);
                }
            }
        }

        internal override bool PrepareHint()
        {
            if (TruncLongText && TextTrunced)
                PanelHint.AddSimpleHint(Text);

            return true;
        }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();

            if ((Image != null) && (Image.ImageIndex >= 0))
                LeftMargin = Image.BitmapList.Size.Width + FormMain.Config.GridSize;
        }

        internal void SetWidthByText()
        {
            Width = Font.WidthText(Text);
        }
    }
}
