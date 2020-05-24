using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    internal enum ResultBattle { Win, Lose, Draw };

    internal sealed class HeroInBattle
    {
        public HeroInBattle (PlayerHero ph)
        {
            PlayerHero = ph;
            Parameters = new HeroParameters(ph.ParametersInBattle);
        }

        internal PlayerHero PlayerHero { get; }
        internal HeroParameters Parameters { get; }
    }       

    // Класс шага сражения
    internal sealed class StepOfBattle
    {
        public StepOfBattle(int step, List<PlayerHero> heroes)
        {
            Step = step;

            foreach (PlayerHero ph in heroes)
                Heroes.Add(new HeroInBattle(ph));
        }

        internal int Step { get; }
        internal List<HeroInBattle> Heroes = new List<HeroInBattle>();
    }

    // Класс результата сражения между игроками
    internal sealed class CourseBattle
    {
        public CourseBattle(Player p1, Player p2, int turn)
        {
            Player1 = p1;
            Player2 = p2;
            Turn = turn;
        }

        internal List<StepOfBattle> Steps { get; } = new List<StepOfBattle>();

        internal void AddStep(int step, List<PlayerHero> heroes)
        {
            Steps.Add(new StepOfBattle(step, heroes));
        }

        internal void AddLog(int step, string text)
        {
            LogBattle += text + Environment.NewLine;
        }

        internal void EndBattle(int step, Player winner)
        {
            //Steps = step;
            Winner = winner;

/*            // Подсчитываем статистику
            foreach (SquadInBattle s in SquadsPlayer1)
            {
                Player1Damage += s.Damaged;
                Player1Kill += s.Killed;
                Player1KillSquad += s.UnitsAlive == 0 ? 1 : 0;
            }

            foreach (SquadInBattle s in SquadsPlayer2)
            {
                Player2Damage += s.Damaged;
                Player2Kill += s.Killed;
                Player2KillSquad += s.UnitsAlive == 0 ? 1 : 0;
            }*/
        }

        internal Player Player1 { get; }
        internal Player Player2 { get; }
        internal Player Winner { get; private set; }
        internal int Turn { get; }
        internal string LogBattle { get; private set; }
        internal int Player1Damage { get; private set; }
        internal int Player1Kill { get; private set; }
        internal int Player1KillSquad { get; private set; }
        internal int Player2Damage { get; private set; }
        internal int Player2Kill { get; private set; }
        internal int Player2KillSquad { get; private set; }
    }
}
