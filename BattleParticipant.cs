using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypePlayer { Human, Computer, Lair };

    // Класс участника битвы
    internal abstract class BattleParticipant
    {
        public BattleParticipant()
        {

        }

        internal string Name { get; set; }
        internal int ImageIndexAvatar { get; set; }
        internal TypePlayer TypePlayer { get; set; }
        internal bool BattleCalced { get; set; } = false;
        internal bool IsLive { get; set; } = true;/*private set*/
        internal List<Battle> HistoryBattles { get; } = new List<Battle>();
        internal List<PlayerHero> CombatHeroes { get; } = new List<PlayerHero>();
    }
}
