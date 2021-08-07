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
        public PlayerCellMenu(PlayerMapObject c, TypeCellMenu r)
        {
            Construction = c;
            Research = r;
        }

        internal PlayerMapObject Construction { get; }
        internal TypeCellMenu Research { get; }

        internal int Cost()
        {
            return Research.Cost;
        }

        internal bool CheckRequirements()
        {
            return Construction.CheckRequirementsForResearch(this);
        }

        internal List<TextRequirement> GetTextRequirements()
        {
            return Construction.GetTextRequirements(this);
        }

        internal void DoResearch()
        {
            Construction.ResearchCompleted(this);

            Program.formMain.SetNeedRedrawFrame();
        }
    }
}
