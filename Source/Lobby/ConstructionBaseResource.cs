using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionBaseResource : EntityForConstruction
    {
        public ConstructionBaseResource(Construction construction, DescriptorBaseResource br) : base(construction, br)
        {
            DescriptorBaseResource = br;
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

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Header(Descriptor.Name, GetImageIndex());
            panelHint.AddStep3Type(Descriptor.GetTypeEntity());
            panelHint.AddStep5Description(Descriptor.Description);
            panelHint.AddStep4Level($"+{Quantity} в день");
            //panelHint.AddStep10CostGold(GetCostGold());
        }
    }
}
