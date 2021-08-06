using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class PanelMonsterInfo : PanelCreatureInfo
    {
        public PanelMonsterInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
        }

        internal Monster Monster { get => PlayerObject as Monster; }

        internal override void Draw(Graphics g)
        {
            lvGold.Text = Monster.TypeMonster.TypeReward.Gold.ToString();

            base.Draw(g);
        }
    }
}
