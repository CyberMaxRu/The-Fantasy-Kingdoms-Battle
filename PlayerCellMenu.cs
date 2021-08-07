using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class PlayerCellMenu
    {
        public PlayerCellMenu(PlayerConstruction c, TypeCellMenu r)
        {
            ObjectOfMap = c;
            Research = r;
        }

        internal PlayerConstruction ObjectOfMap { get; }
        internal TypeCellMenu Research { get; }

        internal int Cost()
        {
            return Research.Cost;
        }

        internal bool CheckRequirements()
        {
            return ObjectOfMap.CheckRequirementsForResearch(this);
        }

        internal List<TextRequirement> GetTextRequirements()
        {
            return ObjectOfMap.GetResearchTextRequirements(this);
        }

        internal void DoResearch()
        {
            ObjectOfMap.ResearchCompleted(this);

            Program.formMain.SetNeedRedrawFrame();
        }
    }
}
