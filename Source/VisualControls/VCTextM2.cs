using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - текст

    internal class VCTextM2 : VisualControl
    {
        private Bitmap bmpPreparedText;
        private string preparedText;

        public VCTextM2(VisualControl parent, int shiftX, int shiftY, M2Font font, Color foreColor, int width) : base(parent, shiftX, shiftY)
        {
            Width = width;
            Font = font;
            Color = foreColor;

            StringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            };
        }

        internal string Text { get; set; }
        internal M2Font Font { get; set; }
        internal Color Color { get; set; }
        internal StringFormat StringFormat { get; set; }

        internal override void Draw(Graphics g)
        {
            if (Text.Length > 0)
            {
                if (preparedText != Text)
                {
                    bmpPreparedText?.Dispose();
                    bmpPreparedText = Font.GetBitmap(Text, Color);
                    preparedText = Text;
                }
            }

            base.Draw(g);

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
                        x = Left + Width - bmpPreparedText.Width;
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

                Debug.Assert(y >= Top);

                g.DrawImageUnscaled(bmpPreparedText, x, y);
            }
        }
    }
}