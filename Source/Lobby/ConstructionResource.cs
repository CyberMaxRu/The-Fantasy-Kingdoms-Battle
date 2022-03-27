using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionResource : EntityForConstruction
    {
        public ConstructionResource(Construction construction, DescriptorResource descriptor) : base(construction, descriptor)
        {
            DescriptorResource = descriptor;
        }

        internal DescriptorResource DescriptorResource { get; }

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Header(Descriptor.Name, GetImageIndex());
            panelHint.AddStep3Type(Descriptor.GetTypeEntity());
            panelHint.AddStep5Description(Descriptor.Description);
        }
    }
}
