using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Класс свойства существа
    internal sealed class CreatureProperty : CreaturePropertyMain
    {
        public CreatureProperty(Creature creature, DescriptorProperty pc) : base(creature)
        {
            Property = pc;
        }

        internal DescriptorProperty Property { get; }
        internal List<Perk> ListSource { get; } = new List<Perk>();
    }
}
