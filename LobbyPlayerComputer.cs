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
            base.SelectStartBonus();
            
            ApplyStartBonus(VariantsStartBonuses[Lobby.Rnd.Next(VariantsStartBonuses.Count)]);
        }

        internal override void DoTurn()
        {
            Debug.Assert(Player.TypePlayer == TypePlayer.Computer);
            Debug.Assert(IsLive || (DayOfEndGame == Lobby.Day - 1));

            // На прошлом ходу игрок потерпел последнее поражение. Выходим
            if (DayOfEndGame > 0)
                return;

            Lobby.StateLobby = StateLobby.TurnComputer;

            // Здесь расчет хода для ИИ
            // Покупаем четыре гильдии и строим 16 героев. На этом пока всё
            GetPlayerConstruction(FormMain.Config.FindTypeGuild("GuildWarrior")).BuyOrUpgrade();
            GetPlayerConstruction(FormMain.Config.FindTypeGuild("GuildRogue")).BuyOrUpgrade();
            GetPlayerConstruction(FormMain.Config.FindTypeGuild("GuildHunter")).BuyOrUpgrade();
            GetPlayerConstruction(FormMain.Config.FindTypeGuild("GuildCleric")).BuyOrUpgrade();
            GetPlayerConstruction(FormMain.Config.FindTypeGuild("GuildMage")).BuyOrUpgrade();

            HireHeroes(GetPlayerConstruction(FormMain.Config.FindTypeGuild("GuildWarrior")), 4);
            HireHeroes(GetPlayerConstruction(FormMain.Config.FindTypeGuild("GuildHunter")), 4);
            HireHeroes(GetPlayerConstruction(FormMain.Config.FindTypeGuild("GuildCleric")), 4);
            HireHeroes(GetPlayerConstruction(FormMain.Config.FindTypeGuild("GuildMage")), 4);

            System.Windows.Forms.Application.DoEvents();
            System.Threading.Thread.Sleep(200);
            System.Windows.Forms.Application.DoEvents();

            void HireHeroes(PlayerConstruction bp, int quantity)
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
                        bp.HireHero();
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
    }
}
