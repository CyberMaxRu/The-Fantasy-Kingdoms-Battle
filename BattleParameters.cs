using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    // Класс параметров героя во время боя
    internal sealed class BattleParameters
    {

        internal bool IsLive { get; set; }// Герой жив
        internal int StepsInTumbstone { get; set; }// Сколько шагов боя герой уже в состоянии могилы
        internal int StepsInResurrection { get; set; }// Сколько шагов боя герой уже воскрешается
    }
}
