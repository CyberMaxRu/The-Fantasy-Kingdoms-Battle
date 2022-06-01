using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class BaseResource : SmallEntity
    {
        public BaseResource(DescriptorBaseResource descriptor) : base(descriptor)
        {
            Descriptor = descriptor;
        }

        internal new DescriptorBaseResource Descriptor { get; }
        internal int Quantity { get; set; }

        internal override int GetImageIndex() => Descriptor.ImageIndex;

        internal override void PrepareHint(PanelHint panelHint)
        {
            
        }
    }
}
