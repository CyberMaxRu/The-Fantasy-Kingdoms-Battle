using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal enum ResultBattle { Win, Draw, Lose, None };

    // Класс боя между двумя игроками
    internal sealed class Battle
    {
        internal Battlefield Battlefield;
        private List<HeroInBattle> heroesForDelete = new List<HeroInBattle>();

        internal Battle(Player player1, Player player2, int turn, Random r, bool showForPlayer)
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
            Rnd = r;

            BattleCalced = false;
            Step = 0;
            SizeBattlefield = new Size(FormMain.Config.HeroRows * 2, FormMain.Config.HeroInRow);            
            Battlefield = new Battlefield(SizeBattlefield.Width, SizeBattlefield.Height);

            // Запоминаем героев в одном списке для упрощения расчетов
            foreach (PlayerHero ph in player1.CombatHeroes)
            {
                AddHero(new HeroInBattle(this, ph, new Point(FormMain.Config.HeroRows - ph.CoordInPlayer.Y - 1 - 3, ph.CoordInPlayer.X), showForPlayer));
            }

            foreach (PlayerHero ph in player2.CombatHeroes)
            {
                AddHero(new HeroInBattle(this, ph, new Point(ph.CoordInPlayer.Y + FormMain.Config.HeroRows + 3, ph.CoordInPlayer.X), showForPlayer));
            }

            void AddHero(HeroInBattle hb)
            {
                Debug.Assert(hb.IsLive == true);
                //Debug.Assert(ph.ParametersInBattle.CurrentHealth > 0);
                //Debug.Assert(Battlefield.Tiles[hb.Coord.Y, hb.Coord.X].Unit == null);

                ActiveHeroes.Add(hb);
                AllHeroes.Add(hb);
                //Battlefield.Tiles[hb.Coord.Y, hb.Coord.X].Unit = hb;
            }
        }

        internal Player Player1 { get; }// Игрок №1        
        internal Player Player2 { get; }// Игрок №2
        internal int Turn { get; }// Ход, на котором произошел бой
        internal Random Rnd { get; }
        internal Size SizeBattlefield { get;  }
        internal int Step { get; private set; }// Шаг боя
        internal bool BattleCalced { get; private set; }
        internal List<HeroInBattle> AllHeroes = new List<HeroInBattle>();// Все участники боя
        internal List<HeroInBattle> ActiveHeroes = new List<HeroInBattle>();// Оставшиеся в живых участники боя
        internal List<Missile> Missiles = new List<Missile>();// Снаряды героев
        internal List<Missile> deleteMissiles = new List<Missile>();// Удаляемые снаряды героев
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
            if ((ActiveHeroes.Where(h => (h.PlayerHero.Player == Player1) && (h.State != StateHeroInBattle.Tumbstone)).Count() == 0) || (ActiveHeroes.Where(h => (h.PlayerHero.Player == Player2) && (h.State != StateHeroInBattle.Tumbstone)).Count() == 0) || (Step == FormMain.Config.MaxStepsInBattle))
            {
                CalcEndBattle();

                return false;
            }

            // Делаем расчет параметров (бафы/дебаффы и прочее)
            foreach (HeroInBattle hb in ActiveHeroes)
                hb.CalcParameters();

            // Делаем действие каждым живым героем
            foreach (HeroInBattle hb in ActiveHeroes)
            {
                // Для отладки проверяем, что нет героев, стоящих на одной клетке
                foreach (HeroInBattle hb2 in ActiveHeroes)
                    if (hb2 != hb)
                        if ((hb2.CurrentTile != null) && (hb.CurrentTile != null) && (hb2.Coord.Equals(hb.Coord)))
                            throw new Exception("Два героя стоят на " + hb2.Coord.ToString());

                hb.DoStepBattle(this);
            }

            // Обрабатываем летающие снаряды
            foreach (Missile m in Missiles)
            {
                m.ApplyStep();
                if (m.StepsPassed == m.StepsToTarget)
                    deleteMissiles.Add(m);
            }

            foreach (Missile m in deleteMissiles)
            {
                Missiles.Remove(m);
            }
            deleteMissiles.Clear();

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

            // Определяем результат боя
            if (Step < FormMain.Config.MaxStepsInBattle)
            {
                int heroesPlayer1 = ActiveHeroes.Where(h => (h.PlayerHero.Player == Player1) && (h.State != StateHeroInBattle.Tumbstone)).Count();
                int heroesPlayer2 = ActiveHeroes.Where(h => (h.PlayerHero.Player == Player2) && (h.State != StateHeroInBattle.Tumbstone)).Count();

                if ((heroesPlayer1 > 0) && (heroesPlayer2 == 0))
                    Winner = Player1;
                else if ((heroesPlayer1 == 0) && (heroesPlayer2 > 0))
                    Winner = Player2;
            }

            if (Winner == Player1)
            {
                ApplyWinAndLose(Player1, Player2);
            }
            else if (Winner == Player2)
            {
                ApplyWinAndLose(Player2, Player1);
            }
            else
            {
                Player1.ResultLastBattle = ResultBattle.Draw;
                Player1.LastBattleDamageToCastle = 0;
                Player2.ResultLastBattle = ResultBattle.Draw;
                Player2.LastBattleDamageToCastle = 0;
            }

            Player1.HistoryBattles.Add(this);
            Player2.HistoryBattles.Add(this);

            Player1.BattleCalced = true;
            Player2.BattleCalced = true;

            void ApplyWinAndLose(Player winner, Player loser)
            {
                winner.ResultLastBattle = ResultBattle.Win;
                loser.ResultLastBattle = ResultBattle.Lose;

                winner.LastBattleDamageToCastle = DamageToCastle();
                loser.LastBattleDamageToCastle = -DamageToCastle();
                loser.DurabilityCastle -= DamageToCastle();
            }

            int DamageToCastle()
            {
                int damage = 0;

                foreach (HeroInBattle h in ActiveHeroes.Where(h => h.PlayerHero.Player == Winner))
                {
                    damage += h.PlayerHero.ClassHero.DamageToCastle;
                }

                Debug.Assert(damage > 0);

                return damage;
            }
        }
    }
}
