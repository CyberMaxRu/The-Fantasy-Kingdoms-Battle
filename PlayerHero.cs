using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс героя игрока
    internal sealed class PlayerHero
    {
        public PlayerHero(PlayerGuild pg)
        {
            Guild = pg;
            Hero = Guild.Guild.TrainedHero;
        }

        internal PlayerGuild Guild { get; }        
        internal Hero Hero { get; }

        internal PanelHero Panel { get; set; }

        internal void ShowDate()
        {
            Debug.Assert(Panel != null);

            Panel.ShowData();
        }
    }
}
