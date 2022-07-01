using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;


namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - текст

    internal class VCText : VisualControl
    {
        private string text;
        private Color color;
        private Padding padding;
        private string preparedText;
        private Padding preparedPadding;
        private List<string> linesText = new List<string>();
        private List<Bitmap> listBitmapPreparedText = new List<Bitmap>();
        private int heightBitmaps;

        public VCText(VisualControl parent, int shiftX, int shiftY, M2Font font, Color foreColor, int width) : base(parent, shiftX, shiftY)
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

        internal string Text { get => text; set { text = value; TextToLines(text); DrawText(); } }
        internal M2Font Font { get; }
        internal Color Color { get; set; }
        internal StringFormat StringFormat { get; set; }
        internal Padding Padding { get => padding; set { padding = value; TextToLines(text); DrawText(); } }
        internal int MinHeigth()
        {
            return linesText.Count > 0 ? Padding.Top + Font.MaxHeightSymbol * linesText.Count + Padding.Bottom : 0;
        }

        internal int MinWidth()
        {
            int w = 0;
            foreach (Bitmap b in listBitmapPreparedText)
                w = Math.Max(w, b.Width);

            return Padding.Left + w + Padding.Right;
        }

        internal void DrawText()
        {
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
            preparedPadding = Padding;
        }

        internal override void Draw(Graphics g)
        {
            if (Text.Length > 0)
            {
                if ((preparedText != Text) || !preparedPadding.Equals(Padding) || (color != Color))
                {
                    color = Color;
                    TextToLines(Text);
                    DrawText();
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
                        y = Top + Padding.Top;
                        break;
                    case StringAlignment.Center:
                        y = Top + Padding.Top + ((Height - Padding.Top - Padding.Bottom - heightBitmaps) / 2);
                        break;
                    default:
                        y = Top + Height - heightBitmaps - Padding.Bottom;
                        break;
                }

                foreach (Bitmap b in listBitmapPreparedText)
                {
                    int x;
                    switch (StringFormat.Alignment)
                    {
                        case StringAlignment.Near:
                            x = Left + Padding.Left;
                            break;
                        case StringAlignment.Center:
                            x = Left + Padding.Left + ((Width - Padding.Left - Padding.Right - b.Width) / 2);
                            break;
                        default:
                            x = Left + Width - b.Width - Padding.Right;
                            break;
                    }

                    Debug.Assert(x >= Left);
                    Debug.Assert(y >= Top);
                    Debug.Assert(b.Width <= Width - Padding.Left - Padding.Right);

                    g.DrawImageUnscaled(b, x, y);

                    y += b.Height;
                }
            }
        }

        private void TextToLines(string text)
        {
            linesText.Clear();
            if (!string.IsNullOrEmpty(text))
            {
                string[] lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < lines.Length; i++)
                    TextLineToLines(lines[i]);
            }
        }

        private void TextLineToLines(string text)
        {
            Debug.Assert(text.Length > 0);

            int posSpace;
            int widthLine;
            int priorPosSpace = -1;
            string tmpStr;
            int actualWidth = Width - Padding.Left - Padding.Right;

            // Разбиваем текст по строкам, учитывая ширину себя
            // Для этого ходим по пробелам
            for (; ; )
            {
                posSpace = text.IndexOf(' ', priorPosSpace + 1);
                if (posSpace > -1)
                {
                    widthLine = Font.WidthText(text.Substring(0, posSpace));
                    // Если превысили свою ширину, то текущая строка - готова
                    if (widthLine > actualWidth)
                    {
                        Debug.Assert(priorPosSpace >= 0, $"{text}, {priorPosSpace} = {priorPosSpace}, Width = {actualWidth}");
                        tmpStr = text.Substring(0, priorPosSpace);
                        linesText.Add(tmpStr);
                        text = text.Substring(priorPosSpace + 1);
                        posSpace = -1;
                    }
                }
                else
                {
                    // Текст закончился. Если остаток влезает в ширину, берем его, иначе отрезаем по пробелу
                    widthLine = Font.WidthText(text);
                    if (widthLine > actualWidth)
                    {
                        tmpStr = text.Substring(0, priorPosSpace);
                        linesText.Add(tmpStr);
                        text = text.Substring(priorPosSpace + 1);
                        widthLine = Font.WidthText(text);
                    }

                    Debug.Assert(widthLine <= actualWidth);
                    Debug.Assert(text.Length > 0);
                    linesText.Add(text);
                    break;
                }

                priorPosSpace = posSpace;
            }

            // Если последней строкой перенос строки, убираем его
            if (linesText[linesText.Count - 1] == Environment.NewLine)
                linesText.RemoveAt(linesText.Count - 1);

            Debug.Assert(linesText.Count > 0);
        }
    }
}