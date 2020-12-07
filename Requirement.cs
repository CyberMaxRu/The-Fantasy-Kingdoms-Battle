using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс требования
    internal sealed class Requirement
    {
        private string nameBuilding;

        public Requirement(string nameRequiredBuilding, int l)
        {
            Debug.Assert(nameRequiredBuilding.Length > 0);
            Debug.Assert(l >= 0);

            nameBuilding = nameRequiredBuilding;
            Level = l;
        }

        internal Building Building { get; private set; }
        internal int Level { get; }

        internal void FindBuilding()
        {
            Building = FormMain.Config.FindBuilding(nameBuilding);
            nameBuilding = null;

            Debug.Assert(Level <= Building.MaxLevel);
        }
    }
}