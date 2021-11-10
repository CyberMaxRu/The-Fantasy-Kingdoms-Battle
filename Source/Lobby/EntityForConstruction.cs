using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class EntityForConstruction : SmallEntity
    {
        public EntityForConstruction(Construction construction, DescriptorSmallEntity descriptor) : base()
        {
            Construction = construction;
            Descriptor = descriptor;
        }

        internal Construction Construction { get; }
        internal DescriptorSmallEntity Descriptor { get; }

    }
}
