using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Потребность сушества
    internal sealed class CreatureNeed : CreaturePropertyMain
    {
        public CreatureNeed(Creature creature, DescriptorCreatureNeed cn) : base(creature)
        {
            Need = cn;

            Value = Creature.BattleParticipant.Lobby.Rnd.Next(cn.MinValueOnHire, cn.MaxValueOnHire + 1);
            IncreasePerDay = cn.IncreasePerDay;
        }

        internal DescriptorCreatureNeed Need { get; }
        internal int IncreasePerDay { get; set; }// Увеличивается каждый день
        internal int Satisfacted { get; set; }// Удовлетворено в прошшлом дне
        internal int DaysMax { get; set; }// Количество дней потребность на максимуме
    }
}
