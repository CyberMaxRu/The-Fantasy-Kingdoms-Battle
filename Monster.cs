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
        public Monster(TypeMonster tm, int level, BattleParticipant bp) : base(tm, bp)
        {
            Debug.Assert(tm != null);
            Debug.Assert(level > 0);

            TypeMonster = tm;
        }

        internal TypeMonster TypeMonster { get; }

        protected override int GetImageIndex()
        {
            //Debug.Assert(IsLive);

            return TypeMonster.ImageIndex;
        }

        internal override void PrepareHint()
        {
            //Debug.Assert(IsLive);

            Program.formMain.formHint.AddStep1Header(TypeMonster.Name, "", TypeMonster.Description);
        }

        internal override void HideInfo()
        {
            Program.formMain.panelMonsterInfo.Visible = false;
        }

        internal override void ShowInfo()
        {
            Debug.Assert(IsLive);

            Program.formMain.panelMonsterInfo.Visible = true;
            Program.formMain.panelMonsterInfo.PlayerObject = this;
        }
    }
}
