using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class CreatureVisitEvent : EntityForCreature
    {
        public CreatureVisitEvent(Creature creature, DescriptorConstructionMassEvent descriptor) : base(creature, descriptor)
        {
            Event = descriptor;
        }

        internal DescriptorConstructionMassEvent Event { get; }

        internal override int GetImageIndex()
        {
            return Event.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            //return Event.
        }
    }
}