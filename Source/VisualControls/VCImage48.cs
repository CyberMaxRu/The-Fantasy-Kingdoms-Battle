using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Класс для иконок 48 * 48
    internal class VCImage48 : VCImage
    {
        public VCImage48(VisualControl parent, int shiftX, int shiftY, int imageIndex) : base(parent, shiftX, shiftY, Program.formMain.imListObjects48, imageIndex)
        {

        }
    }
}
