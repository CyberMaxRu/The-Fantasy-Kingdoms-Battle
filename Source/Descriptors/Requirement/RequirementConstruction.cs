using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class RequirementConstruction : DescriptorRequirement
    {
        private DescriptorConstruction construction;
        private string nameConstruction;
        private int level;
        private int skipTurnsFromBuild;

        public RequirementConstruction(Descriptor forEntity, XmlNode n) : base(forEntity, n)
        {
            nameConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            level = XmlUtils.GetInteger(n, "Level");
            skipTurnsFromBuild = GetInteger(n, "SkipTurnsFromBuild");

            Debug.Assert(nameConstruction.Length > 0);
            Debug.Assert(level >= 0);
            Debug.Assert(skipTurnsFromBuild >= 0);
        }

        public RequirementConstruction(Descriptor forCellMenu, string requiredConstruction, int requiredLevel) : base(forCellMenu)
        {
            nameConstruction = requiredConstruction;
            level = requiredLevel;
        }

        internal override void TuneLinks()
        {
            if (construction != null)
                Utils.DoException($"Уже настроен линк на {construction.ID}.");

            base.TuneLinks();

            construction = Descriptors.FindConstruction(nameConstruction);
            nameConstruction = "";

            Debug.Assert(construction.IsOurConstruction);
            Debug.Assert(level <= construction.MaxLevel, $"Требуется сооружение {construction.ID} {level} уровня, но у него максимум {construction.MaxLevel} уровень.");
        }

        internal override bool CheckRequirement(Player p)
        {
            Construction requiredConstruction = p.GetPlayerConstruction(construction);

            if (ForEntity is DescriptorConstructionLevel dcl)
            {               
                Construction ownerConstruction = p.GetPlayerConstruction(dcl.ActiveEntity as DescriptorConstruction, false);
                // Если владелец еще не построен, то условие не выполнено
                // Такое случается, если речь идет о Храме - его еще нет, а 
                if ((ownerConstruction != null) && (ownerConstruction == requiredConstruction))
                    return (requiredConstruction.Level >= level) && (p.Lobby.Turn - requiredConstruction.DayLevelConstructed[level] >= skipTurnsFromBuild);
                else
                    return p.CheatingIgnoreRequirements ? true : (requiredConstruction.Level >= level) && (p.Lobby.Turn - requiredConstruction.DayLevelConstructed[level] >= skipTurnsFromBuild);
            }
            else
                return p.CheatingIgnoreRequirements ? true : (requiredConstruction.Level >= level) && (p.Lobby.Turn - requiredConstruction.DayLevelConstructed[level] >= skipTurnsFromBuild);
        }

        internal override TextRequirement GetTextRequirement(Player p)
        {
            return new TextRequirement(CheckRequirement(p), p.GetPlayerConstruction(construction).Descriptor.Name + (level > 1 ? " " + level + " уровня" : ""));
        }
    }

}
