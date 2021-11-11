using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class CreatureVisit : EntityForCreature
    {
        public CreatureVisit(Creature creature, DescriptorConstructionVisit descVisit) : base(creature)
        {
            Visit = descVisit;
        }

        internal DescriptorConstructionVisit Visit { get; } 

        internal override int GetImageIndex()
        {
            return Visit.ImageIndex;
        }

        internal override void PrepareHint()
        {
            //return Event.
        }
    }
}
