using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;
using System.Windows.Documents;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class RequirementConstruction : DescriptorRequirement
    {
        private DescriptorConstruction construction;
        private string idConstruction;
        private int skipTurnsFromBuild;

        public RequirementConstruction(Descriptor forEntity, XmlNode n, ListDescriptorRequirements list) : base(forEntity, n, list)
        {
            idConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            Level = XmlUtils.GetInteger(n, "Level");
            skipTurnsFromBuild = GetInteger(n, "SkipTurnsFromBuild");

            Debug.Assert(idConstruction.Length > 0);
            Debug.Assert(Level >= 0);
            Debug.Assert(skipTurnsFromBuild >= 0);
        }

        public RequirementConstruction(Descriptor forCellMenu, string requiredConstruction, int requiredLevel, ListDescriptorRequirements list) : base(forCellMenu, list)
        {
            idConstruction = requiredConstruction;
            Level = requiredLevel;
        }

        internal int Level { get; }

        internal override void TuneLinks()
        {
            if (construction != null)
                Utils.DoException($"Уже настроен линк на {construction.ID}.");

            base.TuneLinks();

            construction = Descriptors.FindConstruction(idConstruction);
            idConstruction = "";

            Debug.Assert(construction.IsOurConstruction);
            Debug.Assert(Level <= construction.MaxLevel, $"Требуется сооружение {construction.ID} {Level} уровня, но у него максимум {construction.MaxLevel} уровень.");

            // Если сущность требует уровень в этого же сооружения, это требование должно идти первым (как самое приоритетное)
            if (ForEntity is DescriptorEntityForActiveEntity de)
                if (de.ActiveEntity is DescriptorConstruction dc)
                    if (dc.ID == construction.ID)
                    {
                        Utils.Assert(List.RequirementOurConstruction is null);

                        if (List.IndexOf(this) != 0)
                            Utils.DoException($"В {de.ID} условие по {dc.ID} должно быть первым (сейчас {List.IndexOf(this)})");

                        List.RequirementOurConstruction = this;
                    }
        }

        internal override bool CheckRequirement(Player p)
        {
            bool allowCheating = base.CheckRequirement(p);

            Construction requiredConstruction = p.GetPlayerConstruction(construction);

            if (ForEntity is DescriptorConstructionLevel dcl)
            {               
                Construction ownerConstruction = p.GetPlayerConstruction(dcl.ActiveEntity as DescriptorConstruction, false);
                // Отдельно обрабатываем случай, когда здание зависит от самого себя - это левелап и читинг в этом случае не допускается,
                // чтобы нельзя было строить новый уровень без построенного предыдущего
                if ((ownerConstruction != null) && (ownerConstruction == requiredConstruction))
                    return (requiredConstruction.Level >= Level) && (p.Lobby.Turn - requiredConstruction.TurnLevelConstructed[Level] >= skipTurnsFromBuild);
                else
                    return allowCheating || ((requiredConstruction.Level >= Level) && (p.Lobby.Turn - requiredConstruction.TurnLevelConstructed[Level] >= skipTurnsFromBuild));
            }
            else
                return allowCheating || ((requiredConstruction.Level >= Level) && (p.Lobby.Turn - requiredConstruction.TurnLevelConstructed[Level] >= skipTurnsFromBuild));
        }

        internal override (bool, string) GetTextRequirement(Player p, Construction inConstruction = null)
        {
            Construction needConstruction = p.GetPlayerConstruction(construction);
            if ((inConstruction != null) && (needConstruction == inConstruction))
                return (CheckRequirement(p), Level == 1 ? "Построить сооружение" : Level.ToString() + "-й уровень сооружения");
            else
                return (CheckRequirement(p), needConstruction.Descriptor.Name + (Level > 1 ? " " + Level + " уровня" : ""));
        }
    }

}
