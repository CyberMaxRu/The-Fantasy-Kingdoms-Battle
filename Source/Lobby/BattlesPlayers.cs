using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Класс результатов битв между игроками
    internal sealed class BattlesPlayers
    {
        public BattlesPlayers(int day)
        {
            Day = day;
        }

        internal int Day { get; }

        internal Dictionary<LobbyPlayer, bool> Players { get; } = new Dictionary<LobbyPlayer, bool>();
    }
}
