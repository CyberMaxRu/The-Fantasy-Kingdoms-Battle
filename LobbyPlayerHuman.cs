﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Игрок-человек
    internal sealed class LobbyPlayerHuman : LobbyPlayer
    {
        public LobbyPlayerHuman(Lobby lobby, Player player, int playerIndex) : base(lobby, player, playerIndex)
        {
            Debug.Assert(player.TypePlayer == TypePlayer.Human);
        }

        internal override void SelectStartBonus()
        {
            // Выбор стартового бонуса
            WindowSelectStartBonus w = new WindowSelectStartBonus();
            w.ShowDialog();
        }
    }
}