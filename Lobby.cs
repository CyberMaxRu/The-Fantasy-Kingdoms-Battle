using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;    

namespace Fantasy_King_s_Battle
{
    // Класс лобби (королевской битвы)
    internal sealed class Lobby
    {
        public Lobby(int quantityPlayers)
        {
            // Создание игроков
            Random r = new Random();
            Players = new Player[quantityPlayers];
            TypePlayer tp;
            for (int i = 0; i < quantityPlayers; i++)
            {
                tp = i == 0 ? TypePlayer.Human : TypePlayer.Computer;
                Players[i] = new Player(this, "Игрок №" + (i + 1).ToString(), FormMain.Config.Fractions[r.Next(0, FormMain.Config.Fractions.Count)], tp);
            }

            ApplyPlayer(0);

            //
            Turn = 1;

            // Определяем противников
            MakeOpponents();
        }

        private void MakeOpponents()
        {
            foreach (Player pl in Players)
                pl.Opponent = null;

            // Алгоритм простой - случайным образом подбираем пару
            List<Player> opponents = new List<Player>();
            opponents.AddRange(Players);
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

        private void ApplyPlayer(int index)
        {
            CurrentPlayerIndex = index;
            CurrentPlayer = Players[CurrentPlayerIndex];
        }

        internal void DoEndTurn()
        {
            // Делаем ходы, перебирая всех игроков, пока все не совершат ход
            for (int i = CurrentPlayerIndex + 1; i < Players.Count(); i++)
            {
                ApplyPlayer(i);

                if (CurrentPlayer.TypePlayer == TypePlayer.Computer)
                    CurrentPlayer.DoTurn();
                else
                    return;
            }


            CalcBattles();

            CalcEndTurn();

            // Делаем начало хода
            Turn++;
            CurrentPlayerIndex = -1;
            MakeOpponents();

            DoEndTurn();
        }

        private void CalcBattles()
        {
            foreach (Player p in Players)
                p.BattleCalced = false;

            foreach (Player p in Players)
            {
                if (p.BattleCalced == false)
                    CalcBattle(p, p.Opponent);
            }
        }

        private void CalcBattle(Player player1, Player player2)
        {
            Debug.Assert(player1.BattleCalced == false);
            Debug.Assert(player2.BattleCalced == false);

            CourseBattle cb = new CourseBattle(player1, player2, Turn);
            Battles.Add(cb);

            Debug.Assert((player1.IsLive == true) && (player2.IsLive == true));
            
            // Инициализируем случайность
            Random r = new Random();

            // Создаем списки действуюих и уничтоженных отрядов
/*            List<SquadInBattle> activeSquad1 = new List<SquadInBattle>();
            List<SquadInBattle> activeSquad2 = new List<SquadInBattle>();
            List<SquadInBattle> lossesSquad1 = new List<SquadInBattle>();
            List<SquadInBattle> lossesSquad2 = new List<SquadInBattle>();

            MakeSquadsInBattle(player1, activeSquad1);
            MakeSquadsInBattle(player2, activeSquad2);

            bool draw = false;
            // Делаем шаги расчета сражения
            for (; ;)
            {
                // Проводим сражение между первыми отрядами
                if (!BattleSquads(activeSquad1[0], activeSquad2[0]))
                {
                    draw = true;
                    break;
                }

                // Если передовой отряд был уничтожен, переносим его в потери
                if (activeSquad1[0].UnitsAlive == 0)
                {
                    lossesSquad1.Add(activeSquad1[0]);
                    activeSquad1.RemoveAt(0);
                }

                if (activeSquad2[0].UnitsAlive == 0)
                {
                    lossesSquad2.Add(activeSquad2[0]);
                    activeSquad2.RemoveAt(0);
                }

                // Переформировываем положение юнитов в отряде
                if (activeSquad1.Count > 0)
                    activeSquad1[0].RearrangeSquad();
                if (activeSquad2.Count > 0)
                    activeSquad2[0].RearrangeSquad();

                // Если у хотя бы одного игрока больше нет отрядов, заканчиваем сражение
                if ((activeSquad1.Count == 0) || (activeSquad2.Count == 0))
                    break;
            }
            
            Player winner = draw == true ? null : (activeSquad1.Count > 0) && (activeSquad2.Count == 0) ? player1 : (activeSquad1.Count == 0) && (activeSquad2.Count > 0) ? player2 : null;

            if (winner == player1)
            {
                player1.Wins++;
                player2.Loses++;
            }
            else if (winner == player2)
            {
                player1.Loses++;
                player2.Wins++;
            }
            else
            {
                player1.Draws++;
                player2.Draws++;
            }

            player1.HistoryBattles.Add(cb);
            player2.HistoryBattles.Add(cb);

            cb.AddLog(0, winner != null ? "Результат сражения. Победитель: " + winner.Name : "Результат сражения. Ничья");
            cb.EndBattle(0, winner);

            player1.BattleCalced = true;
            player2.BattleCalced = true;

            // Записываем результаты сражения
            //MessageBox.Show("Рассчитано шагов: " + step.ToString()
            //    + Environment.NewLine + "Alive: " + Squad1[0].UnitsAlive.ToString() + " - " + Squad2[0].UnitsAlive.ToString()
            //    + Environment.NewLine + "Killed: " + Squad1[0].Killed.ToString() + " - " + Squad2[0].Killed.ToString());

*            // Проводит битву между двумя отрядами
            bool BattleSquads(SquadInBattle s1, SquadInBattle s2)
            {
                //
                cb.AddLog(0, "Начало боя.");
                cb.AddLog(0, s1.GetName() + " vs " + s2.GetName());

                // Делаем шаги расчета сражения
                int step = 1;
                for (; step < Config.MAX_STEP_IN_BATTLE_SQUADS; step++)
                {
                    // Вычисляем повреждение
                    s1.DoDamage(s2, r);
                    s2.DoDamage(s1, r);

                    // Убираем убитых юнитов
                    s1.RemoveDied();
                    s2.RemoveDied();

                    // Если любой из отрядов уничтожен, прекращаем сражение
                    if ((s1.UnitsAlive == 0) || (s2.UnitsAlive == 0))
                        break;
                }

                cb.AddLog(0, "Выполнено шагов: " + step.ToString());
                cb.AddLog(0, "Результат боя: " + ((s1.UnitsAlive > 0) && (s2.UnitsAlive == 0) ? "Победил " + s1.GetName() : ((s2.UnitsAlive > 0) && (s1.UnitsAlive == 0) ? "Победил " + s2.GetName() : "Ничья")));
                cb.AddLog(0, s1.GetName() + ": юнитов " + s1.UnitsAlive.ToString() + "/" + s1.UnitsTotal.ToString());
                cb.AddLog(0, s2.GetName() + ": юнитов " + s2.UnitsAlive.ToString() + "/" + s1.UnitsTotal.ToString());
                cb.AddLog(0, "");

                return step < Config.MAX_STEP_IN_BATTLE_SQUADS;
            }

            // Создает отряды игрока для проведения сражения
            void MakeSquadsInBattle(Player p, List<SquadInBattle> list)
            {
                foreach (Squad s in p.Squads)
                {
                    list.Add(new SquadInBattle(s));
                }
            }*/
        }

        private void CalcEndTurn()
        {
            foreach (Player p in Players)
            {
                p.CalcResultTurn();
            }
        }

        internal CourseBattle GetBattle(Player p, int turn)
        {
            if (turn == 0)
                return null;

            foreach (CourseBattle cb in Battles)
            {
                if (((cb.Player1 == p) || (cb.Player2 == p)) && (cb.Turn == turn))
                    return cb;
            }

            throw new Exception("Сражение не найдено.");
        }

        internal Player[] Players { get; }
        internal int CurrentPlayerIndex { get; private set; }
        internal Player CurrentPlayer { get; private set; }
        internal int Turn { get; private set; }
        internal List<CourseBattle> Battles { get; } = new List<CourseBattle>();
    }
}
