using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCCellSimple : VCImage48
    {
        public VCCellSimple(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, -1)
        {
            ShowBorder = true;
        }

        internal DescriptorEntity Descriptor { get; set; }

        internal override void Draw(Graphics g)
        {
            if (Visible)
            {
                if (Descriptor != null)
                    ImageIndex = Descriptor.ImageIndex;
            }

            base.Draw(g);

            if (ImageIndex == -1)
                DrawImage(g, Program.formMain.bmpEmptyEntity, Left, Top);
        }

        internal override void PaintBorder(Graphics g)
        {
            if (ImageIndex != -1)
                DrawImage(g, Program.formMain.bmpBorderForIcon, Left - 2, Top - 1);
        }
    }
}
