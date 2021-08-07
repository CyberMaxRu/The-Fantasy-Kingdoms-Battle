using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс-обертка над логовом - участник боя
    internal sealed class LairBattleParticipant : BattleParticipant
    {
        public LairBattleParticipant(PlayerConstruction pl) : base(pl.Player.Lobby)
        {
            PlayerLair = pl;
        }

        internal PlayerConstruction PlayerLair { get; }

        internal override string GetName() => PlayerLair.TypeConstruction.Name;
        internal override LobbyPlayer GetPlayer() => PlayerLair.Player;
        internal override TypePlayer GetTypePlayer() => TypePlayer.Lair;
        internal override int GetImageIndexAvatar() => PlayerLair.TypeConstruction.ImageIndex;

        internal override void PreparingForBattle()
        {
            Debug.Assert(!PlayerLair.Destroyed);

            base.PreparingForBattle();
        }

    }
}
