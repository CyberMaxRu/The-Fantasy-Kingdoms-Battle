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
    internal sealed class VCEdit : VCLabelM2
    {
        private Bitmap bmpBackround;

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
        }

        internal override void KeyPress(KeyPressEventArgs e)
        {
            base.KeyPress(e);

            if (((e.KeyChar >= 'a') && (e.KeyChar <= 'z')) || ((e.KeyChar >= 'A') && (e.KeyChar <= 'Z'))
                || ((e.KeyChar >= 'а') && (e.KeyChar <= 'я')) || ((e.KeyChar >= 'А') && (e.KeyChar <= 'Я')))
            {
                if (Text.Length < MaxLength)
                {
                    Text += e.KeyChar;
                    Program.formMain.NeedRedrawFrame();
                }
            }
        }

        internal override void KeyUp(KeyEventArgs e)
        {
            base.KeyUp(e);

            if (e.KeyData == Keys.Back)
            {
                if (Text.Length > 0)
                {
                    Text = Text.Substring(0, Text.Length - 1);
                    Program.formMain.NeedRedrawFrame();
                }
            }
        }
    }
}
