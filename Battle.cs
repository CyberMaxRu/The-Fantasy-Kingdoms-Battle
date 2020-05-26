using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal enum ResultBattle { Win, Lose, Draw };

    // Класс шага сражения
    internal sealed class StepOfBattle
    {
        public StepOfBattle(Battle b, int step, List<HeroInBattle> heroes)
        {
            Step = step;

            Heroes.AddRange(heroes);
        }

        internal int Step { get; }
        internal List<HeroInBattle> Heroes = new List<HeroInBattle>();
    }

    // Класс боя между двумя игроками
    internal sealed class Battle
    {
        internal HeroInBattle[,] battlefield;
        private List<HeroInBattle> heroesForDelete = new List<HeroInBattle>();

        internal Battle(Player player1, Player player2, int turn, Random r)
        {
            Debug.Assert(player1 != null);
            Debug.Assert(player2 != null);
            Debug.Assert(player1 != player2);
            Debug.Assert(player1.BattleCalced == false);
            Debug.Assert(player2.BattleCalced == false);
            Debug.Assert(player1.IsLive == true);
            Debug.Assert(player2.IsLive == true);

            //
            Player1 = player1;
            Player2 = player2;
            Turn = turn;

            int step = 0;
            SizeBattlefield = new Size(Config.HERO_ROWS * 2, Config.HERO_IN_ROW);
            battlefield = new HeroInBattle[SizeBattlefield.Height, SizeBattlefield.Width];

            // Запоминаем героев в одном списке для упрощения расчетов
            foreach (PlayerHero ph in player1.Heroes)
            {
                if ((ph.ClassHero.CategoryHero == CategoryHero.Melee) || (ph.ClassHero.CategoryHero == CategoryHero.Archer) || (ph.ClassHero.CategoryHero == CategoryHero.Mage))
                    AddHero(new HeroInBattle(this, ph, new Point(Config.HERO_ROWS - ph.CoordInPlayer.Y - 1, ph.CoordInPlayer.X)));
            }

            foreach (PlayerHero ph in player2.Heroes)
            {
                if ((ph.ClassHero.CategoryHero == CategoryHero.Melee) || (ph.ClassHero.CategoryHero == CategoryHero.Archer) || (ph.ClassHero.CategoryHero == CategoryHero.Mage))
                    AddHero(new HeroInBattle(this, ph, new Point(ph.CoordInPlayer.Y + Config.HERO_ROWS, ph.CoordInPlayer.X)));
            }

            // Расчет боя
            for (; ; )
            {
                Steps.Add(new StepOfBattle(this, step, ActiveHeroes));

                // Увеличиваем шаг
                step++;

                // Проверяем, окончен ли бой
                // Это либо убиты все герои одной из сторон, либо вышло время боя
                if ((ActiveHeroes.Where(h => h.PlayerHero.Player == player1).Count() == 0) || (ActiveHeroes.Where(h => h.PlayerHero.Player == player2).Count() == 0) || (step == Config.MAX_STEPS_IN_BATTLE))
                    break;

                // Делаем расчет параметров (бафы/дебаффы и прочее)
                foreach (HeroInBattle hb in ActiveHeroes)
                    hb.CalcParameters();

                // Делаем действие каждым живым героем
                foreach (HeroInBattle hb in ActiveHeroes)
                    hb.DoStepBattle(this);

                // Применяем полученный урон, баффы/дебаффы
                foreach (HeroInBattle hb in ActiveHeroes)
                    hb.ApplyStepBattle();

                // Убираем мертвых героев из списка
                Debug.Assert(heroesForDelete.Count == 0);

                foreach (HeroInBattle hb in ActiveHeroes)
                    if (hb.IsLive == false)
                        heroesForDelete.Add(hb);

                foreach (HeroInBattle hb in heroesForDelete)
                    if (ActiveHeroes.Remove(hb) == false)
                        throw new Exception("Герой не был удален из списка.");

                heroesForDelete.Clear();
            }

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
                        }*/

            int res = r.Next(3);
            Player winner = res == 0 ? null : res == 1 ? player1 : player2;

            //Player winner = draw == true ? null : (activeSquad1.Count > 0) && (activeSquad2.Count == 0) ? player1 : (activeSquad1.Count == 0) && (activeSquad2.Count > 0) ? player2 : null;

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

            player1.HistoryBattles.Add(this);
            player2.HistoryBattles.Add(this);

            Winner = winner;

            player1.BattleCalced = true;
            player2.BattleCalced = true;

            // Записываем результаты сражения
            //MessageBox.Show("Рассчитано шагов: " + step.ToString()
            //    + Environment.NewLine + "Alive: " + Squad1[0].UnitsAlive.ToString() + " - " + Squad2[0].UnitsAlive.ToString()
            //    + Environment.NewLine + "Killed: " + Squad1[0].Killed.ToString() + " - " + Squad2[0].Killed.ToString());

            // Проводит битву между двумя отрядами
            /*bool BattleSquads(SquadInBattle s1, SquadInBattle s2)
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

            void AddHero(HeroInBattle hb)
            {
                Debug.Assert(hb.IsLive == true);
                //Debug.Assert(ph.ParametersInBattle.CurrentHealth > 0);
                Debug.Assert(battlefield[hb.Coord.Y, hb.Coord.X] == null);

                ActiveHeroes.Add(hb);
                AllHeroes.Add(hb);
                battlefield[hb.Coord.Y, hb.Coord.X] = hb;
            }
        }

        internal Player Player1 { get; }// Игрок №1        
        internal Player Player2 { get; }// Игрок №2
        internal int Turn { get; }// Ход, на котором произошел бой
        internal Size SizeBattlefield { get;  }
        internal List<StepOfBattle> Steps { get; } = new List<StepOfBattle>();
        internal List<HeroInBattle> AllHeroes = new List<HeroInBattle>();// Все участники боя
        internal List<HeroInBattle> ActiveHeroes = new List<HeroInBattle>();// Оставшиеся в живых участники боя
        internal Player Winner { get; private set; }// Победитель
        internal string LogBattle { get; private set; }
        internal int Player1Damage { get; private set; }
        internal int Player1Kill { get; private set; }
        internal int Player1KillSquad { get; private set; }
        internal int Player2Damage { get; private set; }
        internal int Player2Kill { get; private set; }
        internal int Player2KillSquad { get; private set; }
    }
}
