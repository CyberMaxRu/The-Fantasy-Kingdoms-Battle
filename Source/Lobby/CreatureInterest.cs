using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Интерес существа
    internal sealed class CreatureInterest : CreaturePropertyMain
    {
        public CreatureInterest(Creature creature, DescriptorCreatureInterest ci) : base(creature)
        {
            Descriptor = ci;

            Value = Creature.BattleParticipant.Lobby.Rnd.Next(ci.MinValueOnHire, ci.MaxValueOnHire + 1);
        }

        internal DescriptorCreatureInterest Descriptor { get; }
    }
}