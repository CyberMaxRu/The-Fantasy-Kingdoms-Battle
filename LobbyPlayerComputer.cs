using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Игрок-компьютер
    internal sealed class LobbyPlayerComputer : LobbyPlayer
    {
        public LobbyPlayerComputer(Lobby lobby, Player player, int playerIndex) : base(lobby, player, playerIndex)
        {
            Debug.Assert(player.TypePlayer == TypePlayer.Computer);
        }

        internal override void SelectStartBonus()
        {
            Debug.Assert(VariantsStartBonuses.Count > 0);

            ApplyStartBonus(VariantsStartBonuses[FormMain.Rnd.Next(VariantsStartBonuses.Count)]);
        }
    }
}
