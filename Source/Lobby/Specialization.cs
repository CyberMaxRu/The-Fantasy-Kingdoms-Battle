using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class Specialization : SmallEntity
    {
        public Specialization(Creature c, DescriptorSpecialization type) : base()
        {
            Creature = c;
            TypeSpecialization = type;
        }

        internal Creature Creature { get; }
        internal DescriptorSpecialization TypeSpecialization { get; }

        internal override int GetImageIndex()
        {
            return TypeSpecialization.ImageIndex;
        }

        internal override void PrepareHint()
        {
                        
        }
    }
}
