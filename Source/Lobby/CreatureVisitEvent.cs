using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class CreatureVisitEvent : EntityForCreature
    {
        public CreatureVisitEvent(Creature creature, DescriptorConstructionEvent descriptor) : base(creature)
        {
            Event = descriptor;
        }

        internal DescriptorConstructionEvent Event { get; }

        internal override int GetImageIndex()
        {
            return Event.ImageIndex;
        }

        internal override void PrepareHint()
        {
            //return Event.
        }
    }
}