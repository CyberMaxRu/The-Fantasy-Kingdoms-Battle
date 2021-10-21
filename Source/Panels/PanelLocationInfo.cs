using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class PanelLocationInfo : PanelBaseInfo
    {
        public PanelLocationInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {

        }

        internal Location Location { get => Entity as Location; }

        protected override int GetImageIndex() => Location.Descriptor.ImageIndex;
        protected override bool ImageIsEnabled() => true;// Location.Level > 0;
        protected override string GetCaption() => Location.Descriptor.Name;
    }
}
