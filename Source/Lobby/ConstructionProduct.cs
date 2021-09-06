using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionProduct : SmallEntity
    {
        private DescriptorAbility descriptorAbility;
        private DescriptorItem descriptorItem;
        private DescriptorGroupItem descriptorGroupItem;

        public ConstructionProduct(DescriptorAbility descriptor) : base()
        {
            descriptorAbility = descriptor;
            Descriptor = descriptor;
        }

        public ConstructionProduct(DescriptorItem descriptor) : base()
        {
            descriptorItem = descriptor;
            Descriptor = descriptor;
        }

        public ConstructionProduct(DescriptorGroupItem descriptor) : base()
        {
            descriptorGroupItem = descriptor;
            Descriptor = descriptor;
        }

        internal DescriptorSmallEntity Descriptor { get; }

        internal override string GetCost()
        {
            return "";
        }

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override int GetLevel()
        {
            return 0;
        }

        internal override bool GetNormalImage()
        {
            return true;
        }

        internal override int GetQuantity()
        {
            return 0;
        }

        internal override void PrepareHint()
        {
            
        }
    }
}
