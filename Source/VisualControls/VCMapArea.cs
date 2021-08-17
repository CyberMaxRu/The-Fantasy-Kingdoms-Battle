using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle.Source.VisualControls
{
    // Визуальный контрол - область карты
    internal sealed class VCMapArea : VisualControl
    {
        public VCMapArea(VisualControl parent, int shiftX, int shiftY)
        {
            Tiles = new VCImage[3, 3];


        }

        internal VCImage[,] Tiles { get; }
    }
}
