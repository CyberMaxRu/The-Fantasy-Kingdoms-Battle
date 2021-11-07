using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class EntityForConstruction : SmallEntity
    {
        public EntityForConstruction(Construction construction) : base()
        {
            Construction = construction;
        }

        internal Construction Construction { get; }
        internal DescriptorSmallEntity Descriptor { get; }

    }
}
