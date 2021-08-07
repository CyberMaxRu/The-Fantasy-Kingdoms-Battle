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

            if (r.TypeConstruction != null)
            {
                ConstructionForBuild = c.Player.GetPlayerConstruction(r.TypeConstruction);
            }
        }

        internal PlayerConstruction ObjectOfMap { get; }
        internal TypeCellMenu Research { get; }
        internal PlayerConstruction ConstructionForBuild;

        internal int Cost()
        {
            if (ConstructionForBuild is null)
                return Research.Cost;
            else
                return ConstructionForBuild.CostBuyOrUpgrade();
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

        internal void PrepareHint()
        {
            if (!(Research.Entity is null))
            {
                Program.formMain.formHint.AddStep1Header(Research.Entity.Name, "", Research.Entity.Description);
                Program.formMain.formHint.AddStep3Requirement(GetTextRequirements());
                Program.formMain.formHint.AddStep4Gold(Cost(), Cost() <= ObjectOfMap.Player.Gold);
            }
            else
            {
                ConstructionForBuild.PrepareHintForBuyOrUpgrade();
            }
        }
    }
}
