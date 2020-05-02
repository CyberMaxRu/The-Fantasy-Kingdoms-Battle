using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    // Класс параметров существа
    internal sealed class Parameters
    {
        internal int Strength { get; set; }// Сила
        internal int Dexterity { get; set; }// Ловкость
        internal int Wisdom { get; set; }// Ум
        internal int Stamina { get; set; }
        internal int Speed { get; set; }
        internal int AttackMelee { get; set; }// Умение рукопашной атаки
        internal int AttackRange { get; set; }
        internal int AttackMagic { get; set; }
        internal int DefenseMelee { get; set; }
        internal int DefenseRange { get; set; }
        internal int DefenseMagic { get; set; }
    }
}
