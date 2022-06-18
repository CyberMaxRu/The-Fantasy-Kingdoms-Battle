using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Класс квеста игрока
    internal sealed class PlayerQuest
    {
        public PlayerQuest(Player p, DescriptorMissionQuest quest)
        {
            Player = p;
            Quest = quest;
            TurnActivate = p.Lobby.Turn;
        }

        internal Player Player { get; }
        internal DescriptorMissionQuest Quest { get; }
        internal int TurnActivate { get; }// Ход, на котором квест был активирован
    }
}
