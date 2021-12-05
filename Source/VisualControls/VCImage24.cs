using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCImage24 : VCImage
    {
        public VCImage24(VisualControl parent, int shiftX, int shiftY, int imageIndex) : base(parent, shiftX, shiftY, Program.formMain.ilGui24, imageIndex)
        {
            IsActiveControl = false;
        }
    }
}
