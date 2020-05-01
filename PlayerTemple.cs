using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс храма игрока
    internal sealed class PlayerTemple
    {
        public PlayerTemple(Player p, Temple t)
        {
            Player = p;
            Temple = t;
        }

        internal Player Player { get; }
        internal Temple Temple { get; }
        internal int Level { get; private set; }

        internal void UpdatePanel()
        {
            Temple.Panel.ShowData(this);
        }

        internal void Buy()
        {
            Debug.Assert(Level == 0);

            if (Player.Gold >= Temple.Cost)
            {
                Player.Gold -= Temple.Cost;
                Level = 1;
            }
        }
    }
}