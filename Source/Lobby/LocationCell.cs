using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Класс ячейки локации
    internal sealed class LocationCell : BigEntity
    {
        public LocationCell(Location location) : base()
        {
            Location = location;
        }

        internal Location Location { get; }
        internal DescriptorElementLandscape ElementLandscape { get; set; }
        internal Construction Construction { get; set; }

        internal override void HideInfo()
        {
        }

        internal override void ShowInfo()
        {
        }

        internal override void MakeMenu(VCMenuCell[,] menu)
        {
            
        }

        internal override int GetImageIndex()
        {
            return Construction != null ? Construction.GetImageIndex() : ElementLandscape.ImageIndex;
        }

        internal override void PrepareHint()
        {
        }
    }
}
