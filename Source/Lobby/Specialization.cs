using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class Specialization : EntityForCreature
    {
        public Specialization(Creature creature, DescriptorSpecialization type) : base(creature)
        {
            TypeSpecialization = type;
        }

        internal DescriptorSpecialization TypeSpecialization { get; }

        internal override int GetImageIndex()
        {
            return TypeSpecialization.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
                        
        }
    }
}
