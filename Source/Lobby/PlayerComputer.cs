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

        internal override void DoTurn()
        {
            Debug.Assert(Descriptor.TypePlayer == TypePlayer.Computer);
            Debug.Assert(IsLive || (DayOfEndGame == Lobby.Turn - 1));

            // На прошлом ходу игрок потерпел последнее поражение. Выходим
            if (DayOfEndGame > 0)
                return;

            Lobby.StateLobby = StateLobby.TurnComputer;

            // Здесь расчет хода для ИИ
            // Покупаем гильдии и нанимаем героев. На этом пока всё
            if (Lobby.Turn == 1)
            {
                GetPlayerConstruction(FormMain.Descriptors.FindConstruction("GuildWarrior")).Build(false);
                HireHeroes(GetPlayerConstruction(FormMain.Descriptors.FindConstruction("GuildWarrior")), 4);
            }
            else if (Lobby.Turn == 2)
            {
                //GetPlayerConstruction(FormMain.Config.FindConstruction("GuardBarrack")).Build();
                //HireHeroes(GetPlayerConstruction(FormMain.Config.FindConstruction("GuardBarrack")), 4);
            }
            else if (Lobby.Turn == 3)
            {
                GetPlayerConstruction(FormMain.Descriptors.FindConstruction("GuildHunter")).Build(false);
                HireHeroes(GetPlayerConstruction(FormMain.Descriptors.FindConstruction("GuildHunter")), 4);
            }
            else if (Lobby.Turn == 4)
            {
                GetPlayerConstruction(FormMain.Descriptors.FindConstruction("GuildCleric")).Build(false);
                HireHeroes(GetPlayerConstruction(FormMain.Descriptors.FindConstruction("GuildCleric")), 4);
            }
            /*else if (Lobby.Day == 4)
            {
                GetPlayerConstruction(FormMain.Config.FindConstruction("GuildMage")).Build();
                HireHeroes(GetPlayerConstruction(FormMain.Config.FindConstruction("GuildMage")), 4);
            }*/


            System.Windows.Forms.Application.DoEvents();
            System.Threading.Thread.Sleep(200);
            System.Windows.Forms.Application.DoEvents();

            void HireHeroes(Construction bp, int quantity)
            {
                if (bp.Heroes.Count < bp.MaxHeroes())
                {
                    //int needHire = 49;
                    int needHire = Lobby.Rnd.Next(quantity);

                    for (int x = 0; x < needHire; x++)
                    //                for (; bp.Heroes.Count() < bp.MaxHeroes();)
                    {
                        if (bp.Heroes.Count == bp.MaxHeroes())
                            break;
                        if (CombatHeroes.Count == Lobby.TypeLobby.MaxHeroes)
                            break;
                        //bp.HireHero();
                    }
                }
            }
        }

        internal override void EndTurn()
        {

        }

        internal override void PlayerIsWin()
        {

        }

        internal override void DoTick()
        {
            base.DoTick();
        }
    }
}
