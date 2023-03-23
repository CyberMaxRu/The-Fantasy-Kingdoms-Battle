using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - чекбокс
    internal sealed class VCCheckBox : VCImage
    {
        private readonly VCLabel label;
        private const int IMAGE_INDEX_CHECKED = 0;
        private const int IMAGE_INDEX_UNCHECKED = 1;
        private const int IMAGE_INDEX_HOT = 2;
        private const int IMAGE_INDEX_EMPTY = 3;
        private const int IMAGE_INDEX_FLAG = 4;
        private const int IMAGE_INDEX_UNCHECKED_DISABLED = 5;

        public VCCheckBox(VisualControl parent, int shiftX, int shiftY, string text) : base(parent, shiftX, shiftY, Program.formMain.BmpListCheckBox, 0)
        {
            HighlightUnderMouse = true;
            PlaySoundOnEnter = true;
            PlaySoundOnClick = true;

            label = new VCLabel(this, BitmapList.Size.Width + FormMain.Config.GridSize, 0, Program.formMain.FontParagraph, Color.PaleTurquoise, 24, text);
            label.StringFormat.Alignment = StringAlignment.Near;
            label.StringFormat.LineAlignment = StringAlignment.Center;
            label.SetWidthByText();
            Width = label.ShiftX + label.Width;
        }

        internal bool Checked { get; set; }

        internal override void Draw(Graphics g)
        {
            ImageIndex = Checked ? IMAGE_INDEX_CHECKED : IMAGE_INDEX_UNCHECKED;

            base.Draw(g);
        }

        internal override void MouseUp(Point p)
        {
            base.MouseUp(p);

            Checked = !Checked;
        }
    }
}
