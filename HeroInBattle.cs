using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    // Класс героя во время боя
    internal sealed class HeroInBattle
    {
        public HeroInBattle(PlayerHero ph)
        {
            PlayerHero = ph;
            CurrentParameters = new MainParameters(PlayerHero.ModifiedParameters);
        }
        internal PlayerHero PlayerHero { get; }
        internal MainParameters CurrentParameters { get; }
    }
}
