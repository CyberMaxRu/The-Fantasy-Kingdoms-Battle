using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class CreaturePropertyMain
    {
        public CreaturePropertyMain(Creature c)
        {
            Creature = c;
        }

        internal Creature Creature { get; }
        internal int Value { get; set; }// Текущее значение
    }
}