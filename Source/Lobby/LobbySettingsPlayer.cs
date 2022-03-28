using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypeSelectBonus { Manual, Random };

    // Настройка игрока лобби
    internal sealed class LobbySettingsPlayer
    {
        public LobbySettingsPlayer(DescriptorPlayer player)
        {
            Player = player;
        }

        internal TypePlayer TypePlayer { get; set; }    
        internal DescriptorPlayer Player { get; set; }
        internal TypeSelectBonus TypeSelectPersistentBonus { get; set; }
        internal TypeSelectBonus TypeSelectStartBonus { get; set; }
    }
}
