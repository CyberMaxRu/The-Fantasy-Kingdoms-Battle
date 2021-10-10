using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Потребность сушества
    internal sealed class CreatureNeed
    {
        public CreatureNeed(Creature c, DescriptorCreatureNeed cn)
        {
            Creature = c;
            Need = cn;

            Value = Creature.BattleParticipant.Lobby.Rnd.Next(cn.MinValueOnHire, cn.MaxValueOnHire + 1);
            IncreasePerDay = cn.IncreasePerDay;
        }

        internal Creature Creature { get; }
        internal DescriptorCreatureNeed Need { get; }
        internal int Value { get; set; }// Текущее значение
        internal int IncreasePerDay { get; set; }// Увеличивается каждый день
        internal int Satisfacted { get; set; }// Удовлетворено в прошшлом дне
        internal int DaysMax { get; set; }// Количество дней потребность на максимуме
    }
}
