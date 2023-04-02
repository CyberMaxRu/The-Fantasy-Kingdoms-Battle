using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Игрок-компьютер
    internal sealed class PlayerComputer : Player
    {
        public PlayerComputer(Lobby lobby, DescriptorPlayer player, int playerIndex) : base(lobby, player, playerIndex)
        {
            Debug.Assert(player.TypePlayer == TypePlayer.Computer);
        }

        internal override void SelectStartBonus()
        {
            base.SelectStartBonus();

            SelectRandomPersistentBonus();
            ApplyStartBonus(GetRandomStartBonus());
        }

        internal override void PlayerIsWin()
        {

        }

        internal override void DoTick(bool startNewDay)
        {
            base.DoTick(startNewDay);
        }
    }
}
