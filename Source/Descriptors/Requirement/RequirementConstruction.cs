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

        public RequirementConstruction(DescriptorWithID forEntity, XmlNode n) : base(forEntity, n)
        {
            nameConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            level = XmlUtils.GetInteger(n, "Level");

            Debug.Assert(nameConstruction.Length > 0);
            Debug.Assert(level >= 0);
        }

        public RequirementConstruction(DescriptorWithID forCellMenu, string requiredConstruction, int requiredLevel) : base(forCellMenu)
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

        internal override bool CheckRequirement(Player p) => p.GetPlayerConstruction(construction).Level >= level;
        internal override TextRequirement GetTextRequirement(Player p)
        {
            return new TextRequirement(CheckRequirement(p), p.GetPlayerConstruction(construction).TypeConstruction.Name + (level > 1 ? " " + level + " уровня" : ""));
        }
    }

}
