using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Настройки лобби
    internal sealed class LobbySettings
    {
        public LobbySettings(TypeLobby typeLobby) : base()
        {
            TypeLobby = typeLobby;

            Players = new LobbySettingsPlayer[typeLobby.QuantityPlayers];
            for (int i = 0; i < Players.Length; i++)   
            {
                Players[i] = new LobbySettingsPlayer(FormMain.Descriptors.ComputerPlayers[i]);
            }
        }

        internal TypeLobby TypeLobby { get; }
        internal LobbySettingsPlayer[] Players { get; }
    }
}
