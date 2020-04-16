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
        }

        internal Player Player { get; }
        internal TypeUnit TypeUnit { get; }
        internal PanelSquad PanelSquad { get; set; }
    }
}