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
        public PlayerCellMenu(Construction c, TypeCellMenu r)
        {
            Player = c.Player;
            ObjectOfMap = c;
            Research = r;

            if (r.TypeConstruction != null)
            {
                if (r.TypeConstruction.Category == CategoryConstruction.Temple)
                    ConstructionForBuild = c.Player.GetPlayerConstruction(r.TypeConstruction);
                else if (r.TypeConstruction.Category == CategoryConstruction.External)
                {

                }
                else
                    throw new Exception("Неизвестная категория сооружения: " + r.TypeConstruction.Name);
            }
        }

        internal Player Player { get; }
        internal Construction ObjectOfMap { get; }
        internal TypeCellMenu Research { get; }
        internal Construction ConstructionForBuild;

        internal int Cost()
        {
            if (Research.TypeConstruction is null)
                return Research.Cost;
            else
            {
                if (ConstructionForBuild != null)
                    return ConstructionForBuild.CostBuyOrUpgrade();
                else
                    return Research.TypeConstruction.Levels[1].Cost;
            }
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
            if (!(Research.TypeEntity is null))
            {
                string level = Research.TypeEntity is DescriptorAbility ta ? "Требуемый уровень: " + ta.MinUnitLevel.ToString() : "";
                Program.formMain.formHint.AddStep1Header(Research.TypeEntity.Name, level, Research.TypeEntity.Description);
                Program.formMain.formHint.AddStep3Requirement(GetTextRequirements());
                Program.formMain.formHint.AddStep4Gold(Cost(), Cost() <= ObjectOfMap.Player.Gold);
            }
            else
            {
                if (ConstructionForBuild != null)
                    ConstructionForBuild.PrepareHintForBuildOrUpgrade();
                else
                    Player.PrepareHintForBuildTypeConstruction(Research.TypeConstruction);
            }
        }
    }
}
