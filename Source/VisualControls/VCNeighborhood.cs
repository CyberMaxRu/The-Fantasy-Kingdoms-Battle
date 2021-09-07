using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCNeighborhood : VCImage48
    {
        public VCNeighborhood(VisualControl parent, int shiftX, int shiftY, int imageIndex) : base(parent, shiftX, shiftY, imageIndex)
        {

        }

        internal int Layer { get; set; }
    }
}
