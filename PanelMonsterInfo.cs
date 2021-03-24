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
        public PanelMonsterInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
        }

        internal Monster Monster { get => PlayerObject as Monster; }
    }
}
