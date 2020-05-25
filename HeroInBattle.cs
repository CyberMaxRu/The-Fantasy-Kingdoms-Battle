using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    internal sealed class HeroInBattle
    {
        public HeroInBattle(Battle b, PlayerHero ph)
        {
            Battle = b;
            PlayerHero = ph;
            Parameters = new HeroParameters(ph.ParametersInBattle);
        }

        internal PlayerHero PlayerHero { get; }
        internal HeroParameters Parameters { get; }
        internal Battle Battle { get; }
    }
}
