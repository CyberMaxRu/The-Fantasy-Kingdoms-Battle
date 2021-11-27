using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Интерес существа
    internal sealed class CreatureInterest
    {
        public CreatureInterest(Creature c, DescriptorCreatureInterest ci)
        {
            Creature = c;
            Descriptor = ci;

            Value = Creature.BattleParticipant.Lobby.Rnd.Next(ci.MinValueOnHire, ci.MaxValueOnHire + 1);
        }

        internal Creature Creature { get; }
        internal DescriptorCreatureInterest Descriptor { get; }
        internal int Value { get; set; }// Текущее значение
    }
}