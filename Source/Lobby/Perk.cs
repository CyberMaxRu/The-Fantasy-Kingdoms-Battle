using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class Perk : EntityForCreature
    {
        public Perk(Creature creature, DescriptorPerk descriptor) : base(creature)
        {
            Descriptor = descriptor;
        }

        internal DescriptorPerk Descriptor { get; }

        internal override int GetImageIndex()
        {
            throw new NotImplementedException();
        }

        internal override void PrepareHint()
        {
            throw new NotImplementedException();
        }
    }
}
