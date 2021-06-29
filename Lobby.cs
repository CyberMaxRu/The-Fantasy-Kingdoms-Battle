using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal enum StateLobby { Start, TurnHuman, TurnComputer, CalcTurn };

    // Класс лобби
    internal sealed class Lobby
    {
        private static int generation = 0;
        public Lobby(TypeLobby tl)
        {
            ID = generation++;
            TypeLobby = tl;
            StateLobby = StateLobby.Start;

            // Создание игроков
            Players = new LobbyPlayer[tl.QuantityPlayers];
            Players[0] = new LobbyPlayerHuman(this, Program.formMain.CurrentHumanPlayer, 0);// Живой игрок всегда первый

            // Подбираем компьютерных игроков из пула доступных
            List<ComputerPlayer> listCompPlayers = new List<ComputerPlayer>();
            listCompPlayers.AddRange(FormMain.Config.ComputerPlayers.Where(cp => cp.Active));
            Debug.Assert(listCompPlayers.Count >= TypeLobby.QuantityPlayers - 1);

            int idx;
            for (int i = 1; i < TypeLobby.QuantityPlayers; i++)
            {
                idx = FormMain.Rnd.Next(listCompPlayers.Count);
                Players[i] = new LobbyPlayerComputer(this, listCompPlayers[idx], i);
                listCompPlayers.RemoveAt(idx);
            }

            SortPlayers();

            SetPlayerAsCurrent(0);
            StateLobby = StateLobby.TurnHuman;

            //
            Turn = 1;

            // Определяем противников
            MakeOpponents();
        }

        internal int ID { get; private set; }
        internal TypeLobby TypeLobby { get; }
        internal LobbyPlayer[] Players { get; }
        internal LobbyPlayer CurrentPlayer { get; private set; }
        internal int Turn { get; private set; }        
        internal List<Battle> Battles { get; } = new List<Battle>();
        internal bool HumanIsWin { get; private set; }
        internal StateLobby StateLobby { get; private set; }

        private void MakeOpponents()
        {
            foreach (LobbyPlayer pl in Players)
                pl.Opponent = null;

            // Алгоритм простой - случайным образом подбираем пару
            List<LobbyPlayer> opponents = new List<LobbyPlayer>();
            opponents.AddRange(Players.Where(pp => pp.IsLive));
            Debug.Assert(opponents.Count % 2 == 0);
            Random r = new Random();
            LobbyPlayer p;
            LobbyPlayer oppo;
            for (; ; )
            {
                p = opponents[0];
                oppo = opponents[r.Next(1, opponents.Count)];
                Debug.Assert(p != oppo);
                Debug.Assert(p.Opponent == null);
                Debug.Assert(oppo.Opponent == null);
                p.Opponent = oppo;
                oppo.Opponent = p;
                if (!opponents.Remove(p))
                    throw new Exception("Не смог удалить элемент " + p.Player.Name + " из списка оппонентов");
                if (!opponents.Remove(p.Opponent))
                    throw new Exception("Не смог удалить элемент " + p.Opponent.Player.Name + " из списка оппонентов");

                if (opponents.Count == 0)
                    break;
            }
        }

        private void SetPlayerAsCurrent(int index)
        {
            if (index != -1)
            {
                Debug.Assert(Players[index].IsLive);

                CurrentPlayer = Players[index];
            }
            else
                CurrentPlayer = null;
        }

        internal void StartTurn()
        {
            // Реальный игрок должен быть жив
            Debug.Assert(Players[0].GetTypePlayer() == TypePlayer.Human);
            Debug.Assert(Players[0].IsLive);

            foreach (LobbyPlayer p in Players)                 
            {
                if (p.GetTypePlayer() == TypePlayer.Human)
                {
                    if ((Turn == 1) && (p.VariantsStartBonuses.Count > 0))
                    {
                        p.SelectStartBonus();
                    }

                    return;
                }
            }
        }

        internal void DoEndTurn()
        {
            // Реальный игрок должен быть жив
            Debug.Assert(Players[0].GetTypePlayer() == TypePlayer.Human);
            Debug.Assert(Players[0].IsLive);

            // Делаем ходы, перебирая всех игроков, пока все не совершат ход
            int cpi = CurrentPlayer != null ? CurrentPlayer.PlayerIndex : -1;
            for (int i = cpi + 1; i < Players.Count(); i++)
            {
                if ((Players[i].IsLive == true) || (Players[i].GetTypePlayer() == TypePlayer.Human))
                {
                    SetPlayerAsCurrent(i);

                    if (CurrentPlayer.GetTypePlayer() == TypePlayer.Computer)
                    {
                        StateLobby = StateLobby.TurnComputer;
                        Program.formMain.ShowCurrentPlayerLobby();
                        CurrentPlayer.DoTurn();
                        System.Threading.Thread.Sleep(200);
                    }
                    else
                    {
                        StateLobby = StateLobby.TurnHuman;
                        Program.formMain.ShowCurrentPlayerLobby();
                        return;
                    }
                }
            }

            SetPlayerAsCurrent(-1);
            StateLobby = StateLobby.CalcTurn;
            Program.formMain.ShowCurrentPlayerLobby();
            Program.formMain.ShowNamePlayer("Расчет дня");
            CalcFinalityTurn();

            //CalcBattles();

            CalcResultTurn();

            SortPlayers();

            if (!Players[0].IsLive)
            {
                CurrentPlayer = null;
                return;
            }

            // Делаем начало хода
            Turn++;
            CurrentPlayer = null;

            int livePlayers = 0;
            foreach (LobbyPlayer p in Players)
            {
                if (p.IsLive)
                    livePlayers++;
            }

            if (livePlayers > 1)
                MakeOpponents();
            else
            {
                foreach (LobbyPlayer p in Players)
                {
                    if (p.IsLive)
                    {
                        if (p.GetTypePlayer() == TypePlayer.Human)
                        {
                            HumanIsWin = true;
                        }
                    }
                }

                return;
            }

            DoEndTurn();
        }


        private void CalcBattles()
        {
            Battle b;
            WindowBattle formBattle;

            foreach (LobbyPlayer p in Players)
            {
                p.BattleCalced = false;
            }

            // Бои с монстрами
            foreach (LobbyPlayer p in Players)
            {
                if (p.IsLive)
                {
                    p.PreparingForBattle();

                    // Включить, когда ИИ может выбирать цель
                    //Debug.Assert(p.TargetLair != null);
                    foreach (PlayerLair pl in p.ListFlags)
                    {
                        pl.PreparingForBattle();

                        //Debug.Assert(p.TargetLair.CombatHeroes.Count > 0);

                        bool showForPlayer = p.GetTypePlayer() == TypePlayer.Human;
                        b = new Battle(p, pl, Turn, FormMain.Rnd, showForPlayer);

                        if (showForPlayer)
                        {
                            formBattle = new WindowBattle(b);
                            formBattle.ShowBattle();
                            formBattle.Dispose();
                        }
                        else
                        {
                            //if (formProgressBattle == null)
                            //    formProgressBattle = new FormProgressBattle();

                            //formProgressBattle.SetBattle(b, Players.Count(), p.PlayerIndex + 1);
                            b.CalcWholeBattle();
                        }

                        Battles.Add(b);
                    }
                }
            }

            return;

            int livePlayers = 0;
            foreach (LobbyPlayer p in Players)
            {
                if (p.IsLive)
                    livePlayers++;
            }
            Debug.Assert(livePlayers % 2 == 0);
            int pairs = (livePlayers / 2) - 1;
            int numberPair = 0;

            foreach (LobbyPlayer p in Players)
                p.BattleCalced = false;

            foreach (LobbyPlayer p in Players)
            {
                if (p.IsLive == true)
                {
                    if (p.BattleCalced == false)
                    {
                        bool showForPlayer = (p.GetTypePlayer() == TypePlayer.Human) || (p.Opponent.GetTypePlayer() == TypePlayer.Human);
                        b = new Battle(p, p.Opponent, Turn, FormMain.Rnd, showForPlayer);

                        if (showForPlayer)
                        {
                            formBattle = new WindowBattle(b);
                            formBattle.ShowBattle();
                            formBattle.Dispose();
                        }
                        else
                        {
                            numberPair++;
                            //if (formProgressBattle == null)
                            //    formProgressBattle = new FormProgressBattle();

                            //formProgressBattle.SetBattle(b, pairs, numberPair);
                            b.CalcWholeBattle();
                        }

                        Battles.Add(b);
                    }
                }
            }
        }

        private void CalcBattle(LobbyPlayer player1, LobbyPlayer player2, Random r)
        {
        }

        private void CalcFinalityTurn()
        {
            foreach (LobbyPlayer p in Players)
                if (p.IsLive)
                    p.CalcFinalityTurn();
        }

        private void CalcResultTurn()
        {
            // Делаем расчет итогов дня
            int livePlayers = 0;
            foreach (LobbyPlayer p in Players)
            {
                if (p.IsLive == true)
                {
                    p.CalcResultTurn();
                    livePlayers++;
                }
            }
        }

        internal Battle GetBattle(LobbyPlayer p, int turn)
        {
            if (turn == 0)
                return null;

            foreach (Battle cb in Battles)
            {
                if (((cb.Player1 == p) || (cb.Player2 == p)) && (cb.Turn == turn))
                    return cb;
            }

            throw new Exception("Бой не найден.");
        }

        private void SortPlayers()
        {
            int pos = 1;
            foreach (LobbyPlayer p in Players)
            {
                p.PositionInLobby = pos++;
            }

            // Проверяем, что нет ошибки в позициях
            int curPos = 1;
            foreach (LobbyPlayer p in Players.OrderBy(p => p.PositionInLobby))
            {
                if (p.PositionInLobby != curPos)
                    throw new Exception("Позиция игрока должна быть " + curPos.ToString() + " вместо " + p.PositionInLobby.ToString());

                curPos++;
            }
        }

        internal int DaysForTournament()
        {
            return TypeLobby.DayStartTournament - Turn;
        }

        internal bool CheckUniqueAvatars()
        {
            foreach (LobbyPlayer ph1 in Players)
                foreach (LobbyPlayer ph2 in Players)
                    if (ph1 != ph2)
                        if (ph1.GetImageIndexAvatar() == ph2.GetImageIndexAvatar())
                        {
                            return false;
                        }

            return true;
        }
    }
}
