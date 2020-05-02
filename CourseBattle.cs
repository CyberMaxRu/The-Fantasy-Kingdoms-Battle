using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    internal enum ResultBattle { Win, Lose, Draw };

    // Класс результата сражения между игроками
    internal sealed class CourseBattle
    {
        public CourseBattle(Player p1, Player p2, int turn)
        {
            Player1 = p1;
            Player2 = p2;
            Turn = turn;
        }

        internal void AddLog(int step, string text)
        {
            LogBattle += text + Environment.NewLine;
        }

        internal void EndBattle(int step, Player winner)
        {
            Steps = step;
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
        internal int Steps { get; private set; }
        internal string LogBattle { get; private set; }
        internal int Player1Damage { get; private set; }
        internal int Player1Kill { get; private set; }
        internal int Player1KillSquad { get; private set; }
        internal int Player2Damage { get; private set; }
        internal int Player2Kill { get; private set; }
        internal int Player2KillSquad { get; private set; }
    }
}
