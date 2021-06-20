using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - чекбокс
    internal sealed class VCCheckBox : VCLabelM2
    {
        private const int IMAGE_INDEX_CHECKED = 0;
        private const int IMAGE_INDEX_UNCHECKED = 1;
        private const int IMAGE_INDEX_HOT = 2;
        private const int IMAGE_INDEX_EMPTY = 3;
        private const int IMAGE_INDEX_FLAG = 4;
        private const int IMAGE_INDEX_UNCHECKED_DISABLED = 5;

        public VCCheckBox(VisualControl parent, int shiftX, int shiftY, string text) : base(parent, shiftX, shiftY, Program.formMain.fontParagraph, Color.PaleTurquoise, 24, text)
        {
            StringFormat.Alignment = StringAlignment.Near;
            StringFormat.LineAlignment = StringAlignment.Center;
            Width = 160;
            LeftMargin = 32;

            BitmapList = Program.formMain.blCheckBox;
        }

        internal bool Checked { get; set; }

        internal override void Draw(Graphics g)
        {
            if (MouseEntered)
            {
                if (LeftButtonPressed)
                    ImageIndex = Checked ? IMAGE_INDEX_CHECKED : IMAGE_INDEX_UNCHECKED;
                else
                    ImageIndex = Checked ? IMAGE_INDEX_HOT : IMAGE_INDEX_UNCHECKED;
            }
            else
                ImageIndex = Checked ? IMAGE_INDEX_CHECKED : IMAGE_INDEX_UNCHECKED;

            base.Draw(g);
        }

        internal override void MouseDown()
        {
            base.MouseDown();

            Program.formMain.NeedRedrawFrame();
        }

        internal override void MouseUp()
        {
            base.MouseUp();

            Checked = !Checked;
            Program.formMain.PlayPushButton();
            Program.formMain.NeedRedrawFrame();
        }

        internal override void MouseEnter(bool leftButtonDown)
        {
            base.MouseEnter(leftButtonDown);

            Program.formMain.PlaySelectButton();
            Program.formMain.NeedRedrawFrame();
        }
    }
}
