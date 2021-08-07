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
        public LairBattleParticipant(PlayerLair pl) : base(pl.Player.Lobby)
        {
            PlayerLair = pl;
        }

        internal PlayerLair PlayerLair { get; }

        internal override string GetName() => PlayerLair.TypeLair.Name;
        internal override LobbyPlayer GetPlayer() => PlayerLair.Player;
        internal override TypePlayer GetTypePlayer() => TypePlayer.Lair;
        internal override int GetImageIndexAvatar() => PlayerLair.TypeLair.ImageIndex;

        internal override void PrepareHint()
        {
            Debug.Assert(!PlayerLair.Destroyed);

            if (PlayerLair.Hidden)
                Program.formMain.formHint.AddStep1Header("Неизвестное место", "Место не разведано", "Установите флаг разведки для отправки героев к месту");
            else
            {
                Program.formMain.formHint.AddStep1Header(PlayerLair.TypeLair.Name, "", PlayerLair.TypeLair.Description);
                Program.formMain.formHint.AddStep2Reward(PlayerLair.TypeLair.TypeReward.Gold);
                Program.formMain.formHint.AddStep3Greatness(PlayerLair.TypeLair.TypeReward.Greatness, 0);
            }
        }

        internal override void HideInfo()
        {
            Debug.Assert(!PlayerLair.Destroyed);

            Program.formMain.panelLairInfo.Visible = false;
        }

        internal override void ShowInfo()
        {
            Debug.Assert(!PlayerLair.Destroyed);

            Program.formMain.panelLairInfo.Visible = true;
            Program.formMain.panelLairInfo.PlayerObject = PlayerLair;
        }

        internal override void PreparingForBattle()
        {
            Debug.Assert(!PlayerLair.Destroyed);

            base.PreparingForBattle();
        }

    }
}
