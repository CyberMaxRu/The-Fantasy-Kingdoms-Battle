using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    internal sealed class PlayerResearch
    {
        public PlayerResearch(PlayerBuilding pb, Research r)
        {
            Building = pb;
            Research = r;
        }

        internal PlayerBuilding Building { get; }
        internal Research Research { get; }

        internal int Cost()
        {
            return Research.Cost;
        }
    }
}
