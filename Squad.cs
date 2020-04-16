using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс отряда
    internal sealed class Squad
    {
        public Squad(Player player, TypeUnit typeUnit)
        {
            //Debug.Assert(player.Fraction == typeUnit.Fraction);

            Player = player;
            TypeUnit = typeUnit;

            CalcParameters();
        }

        internal void CalcParameters()
        {
            DamageMin = TypeUnit.DamageMin;
            DamageMax = TypeUnit.DamageMax;
            Health = TypeUnit.Health;
        }

        internal Player Player { get; }
        internal TypeUnit TypeUnit { get; }
        internal PanelSquad PanelSquad { get; set; }

        // Параметры с учетом изменений за счет замка и военачальника
        internal int DamageMin { get; private set; }
        internal int DamageMax { get; private set; }
        internal int Health { get; private set; }
    }
}