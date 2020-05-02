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
            Level = 1;
        }

        internal PlayerGuild Guild { get; }        
        internal Hero Hero { get; }
        internal int Level { get; }

        internal int CurrentHealth;
        internal int MaxHealth;
        internal int CurrentMana;
        internal int MaxMana;
        internal Parameters OurParameters { get; } = new Parameters();
        internal Parameters ModifiedParameters { get; } = new Parameters();
        internal PanelHero Panel { get; set; }

        internal void ShowDate()
        {
            Debug.Assert(Panel != null);

            Panel.ShowData();
        }
    }
}
