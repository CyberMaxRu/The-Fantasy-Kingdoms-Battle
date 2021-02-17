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
            return TypeMonster.ImageIndex;
        }

        protected override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(TypeMonster.Name, "", TypeMonster.Description);
        }

        protected override void DoClick()
        {
            Program.formMain.formHint.AddStep1Header(TypeMonster.Name, "", TypeMonster.Description);
            Program.formMain.SelectMonster(this);
        }
    }
}
