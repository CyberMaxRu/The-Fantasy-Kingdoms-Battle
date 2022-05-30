using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class EntityForConstruction : SmallEntity
    {
        public EntityForConstruction(BigEntity entity, DescriptorSmallEntity descriptor) : base()
        {
            Entity = entity;
            Descriptor = descriptor;

            if (Entity is Construction c)
                Construction = c;

            if (descriptor is DescriptorEntityForActiveEntity ce)
                Selling = new ComponentSelling(ce.Selling);
        }

        internal BigEntity Entity { get; }
        internal Construction Construction { get; }
        internal DescriptorSmallEntity Descriptor { get; }
        internal ComponentSelling Selling { get; }
    }
}
