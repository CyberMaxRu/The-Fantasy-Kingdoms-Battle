using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionProduct : SmallEntity
    {
        public ConstructionProduct(DescriptorAbility descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            DescriptorAbility = descriptor;
            Descriptor = descriptor;
        }

        public ConstructionProduct(DescriptorItem descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            DescriptorItem = descriptor;
            Descriptor = descriptor;
        }

        public ConstructionProduct(DescriptorGroupItems descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            DescriptorGroupItem = descriptor;
            Descriptor = descriptor;
        }

        internal DescriptorSmallEntity Descriptor { get; }
        internal DescriptorAbility DescriptorAbility { get; }
        internal DescriptorItem DescriptorItem { get; }
        internal DescriptorGroupItems DescriptorGroupItem { get; }

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override bool GetNormalImage()
        {
            return true;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Descriptor.Name, "", Descriptor.Description);
        }

        internal bool IsAvailableForCreature(DescriptorCreature dc)
        {
            return Descriptor.AvailableForAllHeroes || (Descriptor.AvailableForHeroes.IndexOf(dc) != -1);
        }
    }
}
