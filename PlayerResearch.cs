using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class PlayerResearch
    {
        public PlayerResearch(PlayerConstruction c, Research r)
        {
            Construction = c;
            Research = r;
        }

        internal PlayerConstruction Construction { get; }
        internal Research Research { get; }

        internal int Cost()
        {
            return Research.Cost;
        }

        internal bool CheckRequirements()
        {
            // Сначала проверяем, построено ли здание
            if (Construction.Level == 0)
                return false;

            // Потом проверяем наличие золота
            if (Construction.Player.Gold < Cost())
                return false;

            // Проверяем, что еще можно делать исследования
            if (!Construction.CanResearch())
                return false;

            // Проверяем требования к исследованию
            return Construction.Player.CheckRequirements(Research.Requirements);
        }

        internal List<TextRequirement> GetTextRequirements()
        {
            List<TextRequirement> list = new List<TextRequirement>();

            if (Construction.Level == 0)
                list.Add(new TextRequirement(false, "Здание не построено"));
            else
            {
                Construction.Player.TextRequirements(Research.Requirements, list);

                if (!Construction.CanResearch())
                    list.Add(new TextRequirement(false, "Больше нельзя выполнять исследований в этот день"));
            }

            return list;
        }

        internal void DoResearch()
        {
            Debug.Assert(CheckRequirements() == true);

            Construction.Player.SpendGold(Cost());
            Construction.Researches.Remove(this);
            Construction.ResearchCompleted();
            AddEntity(Research.Entity);

            Program.formMain.SetNeedRedrawFrame();
        }

        internal void AddEntity(Entity entity)
        {
            Debug.Assert(entity != null);

            Construction.Items.Add(entity);
        }
    }
}
