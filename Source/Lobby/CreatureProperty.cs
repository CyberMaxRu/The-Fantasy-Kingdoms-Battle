using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Класс свойства существа
    internal sealed class CreatureProperty
    {
        public CreatureProperty(DescriptorPropertyCreature pc)
        {
            Property = pc;
        }

        internal DescriptorPropertyCreature Property { get; }
        internal int Value { get; set; }
        internal List<Perk> ListSource { get; } = new List<Perk>();
    }
}
