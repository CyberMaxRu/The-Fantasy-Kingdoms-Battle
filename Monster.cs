using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс монстра в логове
    internal sealed class Monster : Creature
    {
        public Monster(TypeCreature tm, int level, BattleParticipant bp) : base(tm, bp)
        {
            Debug.Assert(tm != null);
            Debug.Assert(level > 0);
        }

        internal override string GetCost() => null;

        internal override void PrepareHint()
        {
            //Debug.Assert(IsLive);

            Program.formMain.formHint.AddStep1Header(TypeCreature.Name, "", TypeCreature.Description);
            Program.formMain.formHint.AddStep2Reward(TypeCreature.TypeReward.Gold);
            Program.formMain.formHint.AddStep3Greatness(TypeCreature.TypeReward.Greatness, 0);
        }

        internal override void HideInfo()
        {
            Program.formMain.panelMonsterInfo.Visible = false;
        }

        internal override void ShowInfo()
        {
            Debug.Assert(IsLive);

            Program.formMain.panelMonsterInfo.PlayerObject = this;
            Program.formMain.panelMonsterInfo.Visible = true;
        }
    }
}
