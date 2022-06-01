using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionBaseResource : EntityForConstruction
    {
        Construction construction;
        public ConstructionBaseResource(BigEntity entity, DescriptorBaseResource br) : base(entity, br)
        {
            DescriptorBaseResource = br;

            if (entity is Construction c)
                construction = c;
        }

        internal DescriptorBaseResource DescriptorBaseResource { get; }
        internal int Quantity { get; set; }

        internal override int GetImageIndex()
        {
            return DescriptorBaseResource.ImageIndex;
        }

        internal override string GetText()
        {
            return Quantity.ToString();
        }

        internal override bool GetNormalImage()
        {
            return construction != null ? construction.MiningBaseResources : true;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Descriptor(Descriptor);
            panelHint.AddStep5Description(Descriptor.Description);
            panelHint.AddStep4Level($"+{Quantity} в день");
            if (construction != null)
                if (!construction.MiningBaseResources && (DescriptorBaseResource.ConstructionForMining != null))
                    panelHint.AddStep5Description("Постройте {" + DescriptorBaseResource.ConstructionForMining.Name + "} для добычи ресурса");
        }
    }
}
