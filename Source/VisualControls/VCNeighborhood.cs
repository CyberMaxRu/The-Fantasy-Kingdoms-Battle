using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCNeighborhood : VCImage
    {
        public VCNeighborhood(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {

        }

        internal int Layer { get; set; }
    }
}
