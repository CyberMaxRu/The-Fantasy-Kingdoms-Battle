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
        private string preparedText;
        private List<string> linesText = new List<string>();
        private List<Bitmap> listBitmapPreparedText = new List<Bitmap>();
        private int heightBitmaps;

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
                    TextToLines(Text);

                    foreach (Bitmap b in listBitmapPreparedText)
                    {
                        b.Dispose();
                    }
                    listBitmapPreparedText.Clear();

                    heightBitmaps = 0;
                    foreach (string s in linesText)
                    {
                        listBitmapPreparedText.Add(Font.GetBitmap(s, Color));
                        heightBitmaps += listBitmapPreparedText[listBitmapPreparedText.Count - 1].Height;
                    }

                    preparedText = Text;
                }
            }

            base.Draw(g);

            if (Text.Length > 0)
            {
                int y;
                // Делаем расчет координаты по вертикали
                switch (StringFormat.LineAlignment)
                {
                    case StringAlignment.Near:
                        y = Top;
                        break;
                    case StringAlignment.Center:
                        y = Top + ((Height - heightBitmaps) / 2);
                        break;
                    default:
                        y = Top + Height - heightBitmaps;
                        break;
                }

                foreach (Bitmap b in listBitmapPreparedText)
                {
                    int x;
                    switch (StringFormat.Alignment)
                    {
                        case StringAlignment.Near:
                            x = Left;
                            break;
                        case StringAlignment.Center:
                            x = Left + ((Width - b.Width) / 2);
                            break;
                        default:
                            x = Left + Width - b.Width;
                            break;
                    }

                    Debug.Assert(x >= Left);
                    Debug.Assert(y >= Top);

                    g.DrawImageUnscaled(b, x, y);

                    y += b.Height;
                }
            }
        }

        private void TextToLines(string text)
        {
            Debug.Assert(text.Length > 0);

            linesText.Clear();

            int posSpace;
            int widthLine;
            int priorPosSpace = -1;
            string tmpStr;
            // Разбиваем текст по строкам, учитывая ширину себя
            // Для этого ходим по пробелам
            for (; ; )
            {
                posSpace = text.IndexOf(' ', priorPosSpace + 1);
                if (posSpace > -1)
                {
                    widthLine = Font.WidthText(text.Substring(0, posSpace));
                    // Если превысили свою ширину, то текущая строка - готова
                    if (widthLine > Width)
                    {
                        Debug.Assert(priorPosSpace >= 0);
                        tmpStr = text.Substring(0, priorPosSpace);
                        linesText.Add(tmpStr);
                        text = text.Substring(priorPosSpace + 1);
                        posSpace = -1;
                    }
                }
                else
                {
                    // Текст закончился. Остаток - последнее слово
                    widthLine = Font.WidthText(text);
                    Debug.Assert(widthLine <= Width);
                    linesText.Add(text);
                    break;
                }

                priorPosSpace = posSpace;
            }

            Debug.Assert(linesText.Count > 0);
        }
    }
}