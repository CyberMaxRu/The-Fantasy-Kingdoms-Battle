using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс навыка военачальника
    internal sealed class SkillOfChieftain
    {
        public SkillOfChieftain(Chieftain c, Skill s)
        {
            Skill = s;
            Level = 1;
            Position = c.Skills.Count;
        }

        internal void LevelUp()
        {
            Debug.Assert(Level > 0);
            Debug.Assert(Level < FormMain.Config.MaxLevelSkill);
            Level++;
        }

        internal Skill Skill { get; }
        internal int Level { get; private set; }
        internal int Position { get; }
    }

    // Класс военачальника
    internal sealed class Chieftain
    {
        public Chieftain(Player player)
        {
            Player = player;
            Experience = 0;
            Level = 1;

            Skills.Add(new SkillOfChieftain(this, FormMain.Config.FindSkill("Leadership")));
            Skills.Add(new SkillOfChieftain(this, FormMain.Config.FindSkill("Luck")));
            Skills[1].LevelUp();
        }

        internal Player Player { get; }
        internal int Experience { get; }
        internal int Level { get; }
        internal List<SkillOfChieftain> Skills { get; } = new List<SkillOfChieftain>();
    }
}
