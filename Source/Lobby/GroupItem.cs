using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class GroupItem : SmallEntity
    {
        public GroupItem(Entity owner, DescriptorGroupItems descriptor) : base(descriptor)
        {
            Owner = owner;
            Descriptor = descriptor;
        }

        internal Entity Owner { get; }
        internal DescriptorGroupItems Descriptor { get; }

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
                        
        }
    }
}
