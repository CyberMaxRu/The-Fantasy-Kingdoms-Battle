using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - метка с иконкой, для отображения значения
    internal sealed class VCLabelValue : VCLabelM2
    {
        public VCLabelValue(VisualControl parent, int shiftX, int shiftY, Color foreColor)
            : base(parent, shiftX, shiftY, Program.formMain.fontMedCaption, foreColor, 16, "")
        {
            BitmapList = Program.formMain.ilGui16;
            StringFormat.Alignment = StringAlignment.Near;
        }
    }
}
