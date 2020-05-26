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

            BattleCalced = false;
            Step = 0;
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
        internal int Step { get; private set; }// Шаг боя
        internal bool BattleCalced { get; private set; }
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

        internal bool CalcStep()
        {
            Debug.Assert(BattleCalced == false);

            // Увеличиваем шаг
            Step++;

            // Проверяем, окончен ли бой
            // Это либо убиты все герои одной из сторон, либо вышло время боя
            if ((ActiveHeroes.Where(h => h.PlayerHero.Player == Player1).Count() == 0) || (ActiveHeroes.Where(h => h.PlayerHero.Player == Player2).Count() == 0) || (Step == Config.MAX_STEPS_IN_BATTLE))
            {
                CalcEndBattle();

                return false;
            }

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

            return true;
        }

        internal void CalcWholeBattle()
        {
            // Полный расчет боя
            for (; ; )
            {
                if (CalcStep() == false)
                    break;
            }
        }

        private void CalcEndBattle()
        {
            BattleCalced = true;
            int res = FormMain.Rnd.Next(3);
            Player winner = res == 0 ? null : res == 1 ? Player1 : Player2;

            //Player winner = draw == true ? null : (activeSquad1.Count > 0) && (activeSquad2.Count == 0) ? player1 : (activeSquad1.Count == 0) && (activeSquad2.Count > 0) ? player2 : null;

            if (winner == Player1)
            {
                Player1.Wins++;
                Player2.Loses++;
            }
            else if (winner == Player2)
            {
                Player1.Loses++;
                Player2.Wins++;
            }
            else
            {
                Player1.Draws++;
                Player2.Draws++;
            }

            Player1.HistoryBattles.Add(this);
            Player2.HistoryBattles.Add(this);

            Winner = winner;

            Player1.BattleCalced = true;
            Player2.BattleCalced = true;
        }
    }
}
