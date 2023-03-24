using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - поле для ввода текста
    internal sealed class VCEdit : VCLabel
    {
        private readonly Stopwatch swFlash = new Stopwatch();
        private int posCursor;
        private int shiftCursor;
        private Pen penCursor = new Pen(Color.White);

        public VCEdit(VisualControl parent, int shiftX, int shiftY, string text, int maxLength)
            : base(parent, shiftX, shiftY, Program.formMain.FontMedCaption, Color.White, 26, text)
        {
            Debug.Assert(maxLength >= 0);
            Debug.Assert(maxLength <= 255);
            StringFormat.Alignment = StringAlignment.Near;
            StringFormat.LineAlignment = StringAlignment.Near;

            MaxLength = maxLength;
            Width = 80;
            TopMargin = 1;
            LeftMargin = 6;
        }

        private void CalcShiftCursor()
        {
            shiftCursor = LeftMargin + Font.WidthText(Text.Substring(0, posCursor));
        }

        internal int MaxLength { get; set; }

        internal override void PaintBorder(Graphics g)
        {
            Program.formMain.bbToolBarLabel.DrawBorder(g, Left, Top, Width, Height, Color.Transparent);
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            if (!VisualLayer.Active && swFlash.IsRunning)
                swFlash.Stop();
            if (VisualLayer.Active && !swFlash.IsRunning)
                swFlash.Start();

            bool cursorShow = (swFlash.ElapsedMilliseconds % (FormMain.Config.FlashCursorTime * 2)) < FormMain.Config.FlashCursorTime;

            if (cursorShow)
            {
                CalcShiftCursor();
                g.DrawLine(penCursor, Left + shiftCursor, Top + 2, Left + shiftCursor, Top + Height - 6);
            }
        }

        internal override void KeyPress(KeyPressEventArgs e)
        {
            base.KeyPress(e);

            if (((e.KeyChar >= 'a') && (e.KeyChar <= 'z')) || ((e.KeyChar >= 'A') && (e.KeyChar <= 'Z'))
                || ((e.KeyChar >= 'а') && (e.KeyChar <= 'я')) || ((e.KeyChar >= 'А') && (e.KeyChar <= 'Я'))
                || ((e.KeyChar >= '0') && (e.KeyChar <= '9'))
                || (e.KeyChar == '.') || (e.KeyChar == '_') || (e.KeyChar == '-')
                || (e.KeyChar == ' '))
            {
                if (Text.Length < MaxLength)
                {
                    Text += e.KeyChar;
                    posCursor++;
                    CalcShiftCursor();
                }
            }
        }

        internal override void KeyUp(KeyEventArgs e)
        {
            base.KeyUp(e);

            if (e.KeyData == Keys.Back)
            {
                if (posCursor > 0)
                {
                    Text = Text.Remove(posCursor - 1, 1);
                    posCursor--;
                    CalcShiftCursor();
                }
            }
            else if (e.KeyData == Keys.Left)
            {
                if (posCursor > 0)
                {
                    posCursor--;
                    CalcShiftCursor();
                }
            }
            else if (e.KeyData == Keys.Right)
            {
                if (posCursor < Text.Length)
                {
                    posCursor++;
                    CalcShiftCursor();
                }
            }
            else if (e.KeyData == Keys.Home)
            {
                posCursor = 0;
                CalcShiftCursor();

            }
            else if (e.KeyData == Keys.End)
            {
                posCursor = Text.Length;
                CalcShiftCursor();
            }
        }

        internal void CursorToEnd()
        {
            posCursor = Text.Length;
        }
    }
}
