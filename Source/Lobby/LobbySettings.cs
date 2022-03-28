using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Настройки лобби
    internal sealed class LobbySettings
    {
        public LobbySettings(TypeLobby typeLobby) : base()
        {
            TypeLobby = typeLobby;

            Players = new LobbySettingsPlayer[typeLobby.QuantityPlayers];
            Players[0] = new LobbySettingsPlayer(Program.formMain.CurrentHumanPlayer);// Игрок-человек всегда первый

            // Подбираем компьютерных игроков из пула доступных
            List<ComputerPlayer> listCompPlayers = new List<ComputerPlayer>();
            listCompPlayers.AddRange(FormMain.Descriptors.ComputerPlayers.Where(cp => cp.Active));
            Assert(listCompPlayers.Count >= TypeLobby.QuantityPlayers - 1);

            int idx;
            Random rnd = new Random();
            for (int i = 1; i < TypeLobby.QuantityPlayers; i++)
            {
                idx = rnd.Next(listCompPlayers.Count);
                Players[i] = new LobbySettingsPlayer(listCompPlayers[idx]);
                listCompPlayers.RemoveAt(idx);
            }
        }

        internal TypeLobby TypeLobby { get; }
        internal LobbySettingsPlayer[] Players { get; }
    }
}
