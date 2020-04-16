using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    // Класс здания, находящегося во владениях игрока
    internal sealed class BuildingOfPlayer
    {
        public BuildingOfPlayer(Building b)
        {
            Building = b;
        }

        internal void CalcCollectResource()
        {
        }

        internal Building Building { get; }
        internal int[] CollectedResource { get; }
        private int[] TotalCollectedResource { get; }// Сделать расчет
    }
}
