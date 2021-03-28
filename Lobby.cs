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

    internal class ComparerPlayerForPosition : IComparer<Player>
    {
        public int Compare(Player p1, Player p2)
        {
            // Сначала сравниваем прочность Замка
            if (p1.DurabilityCastle > p2.DurabilityCastle)
                return 1;

            if (p1.DurabilityCastle < p2.DurabilityCastle)
                return -1;

            // Сравниваем результат последнего боя
            if (p1.ResultLastBattle < p2.ResultLastBattle)
                return 1;

            if (p1.ResultLastBattle > p2.ResultLastBattle)
                return -1;

            // Сравниваем урон при победе/поражении последнего боя
            if ((p1.ResultLastBattle == ResultBattle.Win) && (p2.ResultLastBattle == ResultBattle.Win))
            { 
                if (p1.LastBattleDamageToCastle > p2.LastBattleDamageToCastle)
                    return 1;

                if (p1.LastBattleDamageToCastle < p2.LastBattleDamageToCastle)
                    return -1;
            }

            if ((p1.ResultLastBattle == ResultBattle.Lose) && (p2.ResultLastBattle == ResultBattle.Lose))
            { 
                if (p1.LastBattleDamageToCastle > p2.LastBattleDamageToCastle)
                    return 1;

                if (p1.LastBattleDamageToCastle < p2.LastBattleDamageToCastle)
                    return -1;
            }

            // У игроков одинаковая позиция
            return 0;
        }
    }

    // Класс лобби (королевской битвы)
    internal sealed class Lobby
    {
        private static int generation = 0;
        public Lobby(TypeLobby tl)
        {
            ID = generation++;
            TypeLobby = tl;
            StateLobby = StateLobby.Start;

            // Создание игроков
            Players = new Player[tl.QuantityPlayers];
            TypePlayer tp;
            string namePlayer;
            for (int i = 0; i < TypeLobby.QuantityPlayers; i++)
            {
                tp = i == 0 ? TypePlayer.Human : TypePlayer.Computer;
                namePlayer = tp == TypePlayer.Computer ? "Игрок №" + (i + 1).ToString() : Program.formMain.Settings.NamePlayer;
                Players[i] = new Player(this, i, namePlayer, tp);
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
        internal Player[] Players { get; }
        internal Player CurrentPlayer { get; private set; }
        internal int Turn { get; private set; }        
        internal List<Battle> Battles { get; } = new List<Battle>();
        internal bool HumanIsWin { get; private set; }
        internal StateLobby StateLobby { get; private set; }

        private void MakeOpponents()
        {
            Debug.Assert(QuantityAlivePlayersIsEven());

            foreach (Player pl in Players)
                pl.Opponent = null;

            // Алгоритм простой - случайным образом подбираем пару
            List<Player> opponents = new List<Player>();
            opponents.AddRange(Players.Where(pp => pp.IsLive));
            Debug.Assert(opponents.Count % 2 == 0);
            Random r = new Random();
            Player p;
            Player oppo;
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
                    throw new Exception("Не смог удалить элемент " + p.Name + " из списка оппонентов");
                if (!opponents.Remove(p.Opponent))
                    throw new Exception("Не смог удалить элемент " + p.Opponent.Name + " из списка оппонентов");

                if (opponents.Count == 0)
                    break;
            }
        }

        private void SetPlayerAsCurrent(int index)
        {
            Debug.Assert(Players[index].IsLive || (Players[index].DayOfDie == Turn));

            CurrentPlayer = Players[index];
        }

        internal void DoEndTurn()
        {
            // Реальный игрок должен быть жив
            Debug.Assert(Players[0].IsLive);

            // Делаем ходы, перебирая всех игроков, пока все не совершат ход
            int cpi = CurrentPlayer != null ? CurrentPlayer.PlayerIndex : -1;
            for (int i = cpi + 1; i < Players.Count(); i++)
            {
                if ((Players[i].IsLive == true) || (Players[i].TypePlayer == TypePlayer.Human))
                {
                    SetPlayerAsCurrent(i);

                    if (CurrentPlayer.TypePlayer == TypePlayer.Computer)
                    {
                        StateLobby = StateLobby.TurnComputer;
                        Program.formMain.ShowCurrentPlayerLobby();
                        CurrentPlayer.DoTurn();
                        System.Threading.Thread.Sleep(250);
                    }
                    else
                    {
                        StateLobby = StateLobby.TurnHuman;
                        Program.formMain.ShowCurrentPlayerLobby();
                        return;
                    }
                }
            }

            StateLobby = StateLobby.CalcTurn;
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
            foreach (Player p in Players)
            {
                if (p.IsLive)
                    livePlayers++;
            }

            if (livePlayers > 1)
                MakeOpponents();
            else
            {
                foreach (Player p in Players)
                {
                    if (p.IsLive)
                    {
                        if (p.TypePlayer == TypePlayer.Human)
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
            FormBattle formBattle;
            FormProgressBattle formProgressBattle = null;

            foreach (Player p in Players)
            {
                p.BattleCalced = false;
            }

            // Бои с монстрами
            foreach (Player p in Players)
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

                        bool showForPlayer = p.TypePlayer == TypePlayer.Human;
                        b = new Battle(p, pl, Turn, FormMain.Rnd, showForPlayer);

                        if (showForPlayer)
                        {
                            formBattle = new FormBattle();
                            formBattle.ShowBattle(b);
                            formBattle.Dispose();
                        }
                        else
                        {
                            if (formProgressBattle == null)
                                formProgressBattle = new FormProgressBattle();

                            formProgressBattle.SetBattle(b, Players.Count(), p.PlayerIndex + 1);
                        }

                        Battles.Add(b);
                    }
                }
            }

            return;

            int livePlayers = 0;
            foreach (Player p in Players)
            {
                if (p.IsLive)
                    livePlayers++;
            }
            Debug.Assert(livePlayers % 2 == 0);
            int pairs = (livePlayers / 2) - 1;
            int numberPair = 0;

            foreach (Player p in Players)
                p.BattleCalced = false;

            foreach (Player p in Players)
            {
                if (p.IsLive == true)
                {
                    if (p.BattleCalced == false)
                    {
                        bool showForPlayer = (p.TypePlayer == TypePlayer.Human) || (p.Opponent.TypePlayer == TypePlayer.Human);
                        b = new Battle(p, p.Opponent, Turn, FormMain.Rnd, showForPlayer);

                        if (showForPlayer)
                        {
                            formBattle = new FormBattle();
                            formBattle.ShowBattle(b);
                            formBattle.Dispose();
                        }
                        else
                        {
                            numberPair++;
                            if (formProgressBattle == null)
                                formProgressBattle = new FormProgressBattle();

                            formProgressBattle.SetBattle(b, pairs, numberPair);
                        }

                        Battles.Add(b);
                    }
                }
            }

            if (formProgressBattle != null)
                formProgressBattle.Dispose();
        }

        private void CalcBattle(Player player1, Player player2, Random r)
        {
        }

        private void CalcFinalityTurn()
        {
            foreach (Player p in Players)
                if (p.IsLive)
                    p.CalcFinalityTurn();
        }

        private void CalcResultTurn()
        {
            // Делаем расчет итогов дня
            int livePlayers = 0;
            foreach (Player p in Players)
            {
                if (p.IsLive == true)
                {
                    p.CalcResultTurn();
                    livePlayers++;
                }
            }

            // Смотрим, сколько игроков осталось живо
            // Если их больше одного, то воскрешаем до четного количества
            // Если меньше, то выявился победитель
            if (livePlayers > 2)
            {
                // Если нечетное число, то одного воскрешаем
                // Для этого ищем, кто находился на самом высоком месте и умер в текущий день 
                if (!QuantityAlivePlayersIsEven())
                {
                    foreach (Player p in Players.OrderByDescending(p => p.PositionInLobby))
                        if (p.DayOfDie == Turn)
                        {
                            p.MakeAlive();
                            break;
                        }
                }

                Debug.Assert(QuantityAlivePlayersIsEven());
            }
        }

        internal bool QuantityAlivePlayersIsEven()
        {
            bool evenPlayers = true;
            foreach (Player p in Players)
                if (p.IsLive)
                    evenPlayers = !evenPlayers;

            return evenPlayers;
        }

        internal Battle GetBattle(Player p, int turn)
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
            foreach (Player p in Players.Where(p => (p.IsLive == true) || (p.DayOfDie == Turn)).OrderByDescending(p => p, new ComparerPlayerForPosition()))
            {
                p.PositionInLobby = pos++;
            }

            // Проверяем, что нет ошибки в позициях
            int curPos = 1;
            foreach (Player p in Players.OrderBy(p => p.PositionInLobby))
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
    }
}
