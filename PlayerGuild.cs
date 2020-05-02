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
        internal List<PlayerHero> Heroes { get; } = new List<PlayerHero>();

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

        internal PlayerHero HireHero()
        {
            Debug.Assert(Heroes.Count < Guild.MaxHeroes);
            Debug.Assert(Player.Heroes.Count < FormMain.MAX_HEROES_AT_PLAYER);

            PlayerHero h = new PlayerHero(this);
            Heroes.Add(h);
            Player.Heroes.Add(h);

            return h;
        }

        internal bool CanTrainHero()
        {
            Debug.Assert(Level > 0);
            Debug.Assert(Heroes.Count <= Guild.MaxHeroes);
            Debug.Assert(Player.Heroes.Count <= FormMain.MAX_HEROES_AT_PLAYER);

            return (Heroes.Count < Guild.MaxHeroes) && (Player.Heroes.Count < FormMain.MAX_HEROES_AT_PLAYER);
        }
    }
}
