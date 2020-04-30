using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс гильдии игрока
    internal sealed class PlayerGuild
    {
        public PlayerGuild(Player p, Guild g)
        {
            Player = p;
            Guild = g;
        }
        
        internal Player Player { get;}
        internal Guild Guild { get; }
        internal int Level { get; private set; }

        internal void UpdatePanel()
        {
            Guild.Panel.ShowData(this);
        }

        internal void Buy()
        {
            Debug.Assert(Level == 0);

            if (Player.Gold >= Guild.Cost)
            {
                Player.Gold -= Guild.Cost;
                Level = 1;
            }    
        }
    }
}
