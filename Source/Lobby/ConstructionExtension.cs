using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionExtension : EntityForConstruction
    {
        public ConstructionExtension(Construction construction, DescriptorConstructionExtension descriptor) : base(construction, descriptor)
        {
            Descriptor = descriptor;
        }

        internal new DescriptorConstructionExtension Descriptor { get; }

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Descriptor(Descriptor);
            panelHint.AddStep5Description(Descriptor.Description);
            panelHint.AddStep9ListNeeds(Descriptor.ListNeeds, true);
            panelHint.AddStep9Interest(Descriptor.ModifyInterest, true);
        }
    }
}
