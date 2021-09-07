using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class GroupItem : SmallEntity
    {
        public GroupItem(Entity owner, DescriptorGroupItems descriptor) : base()
        {
            Owner = owner;
            Descriptor = descriptor;
        }

        internal Entity Owner { get; }
        internal DescriptorGroupItems Descriptor { get; }

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
