using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal enum ResultBattle { Win, Draw, Lose, None };

    // Класс боя между двумя игроками
    internal sealed class Battle
    {
        internal Battlefield Battlefield;
        private List<HeroInBattle> heroesForDelete = new List<HeroInBattle>();

        internal Battle(BattleParticipant player1, BattleParticipant player2, int turn, Random r, bool showForPlayer)
        {
            Debug.Assert(player1 != null);
            Debug.Assert(player2 != null);
            Debug.Assert(player1 != player2);
            //Debug.Assert(player1.BattleCalced == false);
            //Debug.Assert(player2.BattleCalced == false);
            Debug.Assert(player1.IsLive == true);
            Debug.Assert(player2.IsLive == true);

            //
            Player1 = player1;
            Player2 = player2;
            Turn = turn;
            Rnd = r;

            BattleCalced = false;
            Step = 0;
            SizeBattlefield = new Size((FormMain.Config.HeroRows * 2) + FormMain.Config.RowsBetweenSides, FormMain.Config.HeroInRow);
            Battlefield = new Battlefield(SizeBattlefield.Width, SizeBattlefield.Height);

            // Составляем списки существ
            if (player2 is PlayerLair pl)
            {
                foreach (PlayerHero ph in pl.listAttackedHero)
                {
                    Debug.Assert(ph.IsLive);
                    heroesPlayer1.Add(new HeroInBattle(this, ph, showForPlayer));
                }
            }
            else
            {
                foreach (Creature ph in player1.CombatHeroes)
                {
                    Debug.Assert(ph.IsLive);
                    heroesPlayer1.Add(new HeroInBattle(this, ph, showForPlayer));
                }
            }

            foreach (Creature ph in player2.CombatHeroes)
            {
                Debug.Assert(ph.IsLive);
                heroesPlayer2.Add(new HeroInBattle(this, ph, showForPlayer));
            }

            ActiveHeroes.AddRange(heroesPlayer1);
            ActiveHeroes.AddRange(heroesPlayer2);
            AllHeroes.AddRange(ActiveHeroes);

            // Распределяем стартовые места существ
            player1.ArrangeHeroes(heroesPlayer1);
            player2.ArrangeHeroes(heroesPlayer2);

            foreach (HeroInBattle hb in heroesPlayer1)
                hb.CurrentTile = Battlefield.Tiles[hb.StartCoord.Y, hb.StartCoord.X];

            foreach (HeroInBattle hb in heroesPlayer2)
                hb.CurrentTile = Battlefield.Tiles[FormMain.Config.HeroRows + FormMain.Config.RowsBetweenSides + (FormMain.Config.HeroRows - hb.StartCoord.X) - 1, hb.StartCoord.Y];
        }

        internal BattleParticipant Player1 { get; }// Сторона №1        
        internal BattleParticipant Player2 { get; }// Сторона №2
        internal int Turn { get; }// Ход, на котором произошел бой
        internal Random Rnd { get; }
        internal Size SizeBattlefield { get; }
        internal int Step { get; private set; }// Шаг боя
        internal bool BattleCalced { get; private set; }
        internal List<HeroInBattle> AllHeroes = new List<HeroInBattle>();// Все участники боя
        internal List<HeroInBattle> ActiveHeroes = new List<HeroInBattle>();// Оставшиеся в живых участники боя
        internal List<HeroInBattle> DeadHeroes = new List<HeroInBattle>();// Убитые участники боя
        internal List<Missile> Missiles = new List<Missile>();// Снаряды героев
        internal List<Missile> deleteMissiles = new List<Missile>();// Удаляемые снаряды героев
        internal BattleParticipant Winner { get; private set; }// Победитель
        internal string LogBattle { get; private set; }
        internal int Player1Damage { get; private set; }
        internal int Player1Kill { get; private set; }
        internal int Player1KillSquad { get; private set; }
        internal int Player2Damage { get; private set; }
        internal int Player2Kill { get; private set; }
        internal int Player2KillSquad { get; private set; }
        internal List<HeroInBattle> heroesPlayer1 { get; } = new List<HeroInBattle>();
        internal List<HeroInBattle> heroesPlayer2 { get; } = new List<HeroInBattle>();

        internal bool CalcStep()
        {
            Debug.Assert(BattleCalced == false);

            // Увеличиваем шаг
            Step++;

            // Проверяем, окончен ли бой - убиты все герои одной из сторон
            bool player1HeroLives = false;
            bool player2HeroLives = false;
            for (int i = 0; i < ActiveHeroes.Count; i++)
            {
                if (player1HeroLives && player2HeroLives)
                    break;

                if (!player1HeroLives)
                    if ((ActiveHeroes[i].Player == Player1) && (ActiveHeroes[i].State != StateHeroInBattle.Tumbstone))
                        player1HeroLives = true;

                if (!player2HeroLives)
                    if ((ActiveHeroes[i].Player == Player2) && (ActiveHeroes[i].State != StateHeroInBattle.Tumbstone))
                        player2HeroLives = true;
            }

            if (!player1HeroLives || !player2HeroLives)
            {
                CalcEndBattle();

                return false;
            }

            // Если все живые, но время вышло, оканчиваем бой
            if (Step == FormMain.Config.MaxStepsInBattle)
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
                if (hb.CurrentTile != null)
                {
                    foreach (HeroInBattle hb2 in ActiveHeroes)
                        if ((hb2 != hb) && (hb2.CurrentTile != null))
                            if ((hb2.Coord.X == hb.Coord.X) && (hb2.Coord.Y == hb.Coord.Y))
                                throw new Exception("Два героя стоят на " + hb2.Coord.ToString());
                }

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
            {
                if (ActiveHeroes.Remove(hb) == false)
                    throw new Exception("Герой не был удален из списка.");

                Debug.Assert(DeadHeroes.IndexOf(hb) == -1);
                DeadHeroes.Add(hb);
            }
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

             //   if (formProgress != null)
             //       formProgress.ShowStep();

                Application.DoEvents();
            }
        }

        private void CalcEndBattle()
        {
            BattleCalced = true;

            // Определяем результат боя
            if (Step < FormMain.Config.MaxStepsInBattle)
            {
                int heroesPlayer1 = ActiveHeroes.Where(h => (h.PlayerHero.BattleParticipant == Player1) && (h.State != StateHeroInBattle.Tumbstone)).Count();
                int heroesPlayer2 = ActiveHeroes.Where(h => (h.PlayerHero.BattleParticipant == Player2) && (h.State != StateHeroInBattle.Tumbstone)).Count();

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
                /* To restore
                Player1.ResultLastBattle = ResultBattle.Draw;
                Player1.LastBattleDamageToCastle = 0;
                Player2.ResultLastBattle = ResultBattle.Draw;
                Player2.LastBattleDamageToCastle = 0;*/
            }

            Player1.HistoryBattles.Add(this);
            Player2.HistoryBattles.Add(this);

            Player1.BattleCalced = true;
            Player2.BattleCalced = true;

            // Если вторая сторона - логово, удаляем убитых монстров
            if (Player2 is PlayerLair pl)
            {
                foreach (HeroInBattle hb in DeadHeroes)
                {
                    if (hb.Player == Player2)
                        pl.MonsterIsDead(hb.PlayerHero as Monster);
                }
            }

            void ApplyWinAndLose(BattleParticipant winner, BattleParticipant loser)
            {
                /* To restore
                winner.ResultLastBattle = ResultBattle.Win;
                loser.ResultLastBattle = ResultBattle.Lose;

                winner.LastBattleDamageToCastle = DamageToCastle();
                loser.LastBattleDamageToCastle = -DamageToCastle();
                loser.DurabilityCastle -= DamageToCastle();*/
            }
        }
    }
}
