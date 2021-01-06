using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    // Класс монстра в логове
    internal sealed class LairMonster
    {
        public LairMonster(Monster m, int level)
        {            
            Monster = m;

        }

        internal Monster Monster { get; }
    }
}
