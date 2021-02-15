using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class PanelMonsterInfo : PanelCreatureInfo
    {
        private Monster monster;
        public PanelMonsterInfo(VisualControl parent, int shiftX, int shiftY, int height) : base(parent, shiftX, shiftY, height)
        {
        }

        internal Monster Monster
        {
            get { return monster; }
            set
            {
                monster = value;
                Creature = monster;
            }
        }
    }
}
