using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс здания игрока
    internal sealed class PlayerBuilding
    {
        public PlayerBuilding(Player p, Building b)
        {
            Player = p;
            Building = b;

            Level = b.DefaultLevel;
        }

        internal Player Player { get; }
        internal Building Building { get; }
        internal int Level { get; private set; }

        internal void UpdatePanel()
        {
            Building.Panel.ShowData(this);
        }

        internal void Buy()
        {
            Debug.Assert(Level == 0);

            if (Player.Gold >= Building.Levels[1].Cost)
            {
                Player.Gold -= Building.Levels[1].Cost;
                Level = 1;
            }
        }
    }
}