﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class CreatureVisit : EntityForCreature
    {
        public CreatureVisit(Creature creature, DescriptorConstructionVisitSimple descVisit) : base(creature, descVisit)
        {
            Visit = descVisit;
        }

        internal DescriptorConstructionVisitSimple Visit { get; } 

        internal override int GetImageIndex()
        {
            return Visit.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            //return Event.
        }
    }
}
