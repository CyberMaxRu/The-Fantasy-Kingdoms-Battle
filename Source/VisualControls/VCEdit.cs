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
        private Bitmap bmpBackround;
        private Timer timerCursor;
        private int posCursor;
        private int shiftCursor;
        private bool cursorShow;
        private Pen penCursor = new Pen(Color.White);

        public VCEdit(VisualControl parent, int shiftX, int shiftY, string text, int maxLength)
            : base(parent, shiftX, shiftY, Program.formMain.fontMedCaption, Color.White, 26, text)
        {
            Debug.Assert(maxLength >= 0);
            Debug.Assert(maxLength <= 255);
            StringFormat.Alignment = StringAlignment.Near;
            StringFormat.LineAlignment = StringAlignment.Near;

            MaxLength = maxLength;
            Width = 80;
            TopMargin = 1;
            LeftMargin = 6;

            timerCursor = new Timer();
            timerCursor.Interval = 400;
            timerCursor.Tick += TimerCursor_Tick;
            timerCursor.Start();
        }

        private void TimerCursor_Tick(object sender, EventArgs e)
        {
            cursorShow = !cursorShow;

            if (cursorShow)
            {
                CalcShiftCursor();
            }

            Program.formMain.ShowFrame(true);
        }

        private void CalcShiftCursor()
        {
            shiftCursor = LeftMargin + Font.WidthText(Text.Substring(0, posCursor));
        }

        internal int MaxLength { get; set; }

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

            if (cursorShow)
            {
                g.DrawLine(penCursor, Left + shiftCursor, Top + 2, Left + shiftCursor, Top + Height - 6);
            }
        }

        internal override void KeyPress(KeyPressEventArgs e)
        {
            base.KeyPress(e);

            if (((e.KeyChar == 'a') && (e.KeyChar <= 'z')) || ((e.KeyChar >= 'A') && (e.KeyChar <= 'Z'))
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
                    Program.formMain.NeedRedrawFrame();
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
                    Program.formMain.NeedRedrawFrame();
                }
            }
            else if (e.KeyData == Keys.Left)
            {
                if (posCursor > 0)
                {
                    posCursor--;
                    CalcShiftCursor();
                    Program.formMain.NeedRedrawFrame();
                }
            }
            else if (e.KeyData == Keys.Right)
            {
                if (posCursor < Text.Length)
                {
                    posCursor++;
                    CalcShiftCursor();
                    Program.formMain.NeedRedrawFrame();
                }
            }
            else if (e.KeyData == Keys.Home)
            {
                posCursor = 0;
                CalcShiftCursor();
                Program.formMain.NeedRedrawFrame();

            }
            else if (e.KeyData == Keys.End)
            {
                posCursor = Text.Length;
                CalcShiftCursor();
                Program.formMain.NeedRedrawFrame();
            }
        }

        internal void CursorToEnd()
        {
            posCursor = Text.Length;
        }
    }
}
