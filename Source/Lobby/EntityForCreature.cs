using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class EntityForCreature : SmallEntity
    {
        public EntityForCreature(Creature creature)
        {
            Creature = creature;
        }

        internal Creature Creature { get; }
    }
}
