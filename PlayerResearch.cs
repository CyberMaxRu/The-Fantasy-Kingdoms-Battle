using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_King_s_Battle
{
    internal sealed class PlayerResearch
    {
        public PlayerResearch(PlayerBuilding pb, Research r)
        {
            Building = pb;
            Research = r;
        }

        internal PlayerBuilding Building { get; }
        internal Research Research { get; }

        internal int Cost()
        {
            return Research.Cost;
        }

        internal bool CheckRequirements()
        {
            // Сначала проверяем, построено ли здание
            if (Building.Level == 0)
                return false;

            // Потом проверяем наличие золота
            if (Building.Player.Gold < Cost())
                return false;

            // Проверяем требования к исследованию
            return Building.Player.CheckRequirements(Research.Requirements);
        }

        internal List<TextRequirement> GetTextRequirements()
        {
            List<TextRequirement> list = new List<TextRequirement>();

            if (Building.Level == 0)
                list.Add(new TextRequirement(false, "Здание не построено"));
            else
                Building.Player.TextRequirements(Research.Requirements, list);

            return list;
        }
    }
}
