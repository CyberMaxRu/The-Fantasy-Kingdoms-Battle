using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class SmallEntity : Entity
    {
        public SmallEntity(DescriptorSmallEntity descriptor) : base()
        {
            Descriptor = descriptor;
        }

        internal DescriptorSmallEntity Descriptor { get; }

        internal override string GetName() => Descriptor.Name;
        internal override string GetTypeEntity() => Descriptor.GetTypeEntity();
    }
}
