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
    internal sealed class RequirementDestroyedLairs : DescriptorRequirement
    {
        private DescriptorConstruction construction;
        private string nameConstruction;
        private int destroyed;

        public RequirementDestroyedLairs(DescriptorSmallEntity forEntity, XmlNode n) : base(forEntity, n)
        {
            nameConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            destroyed = XmlUtils.GetInteger(n, "Destroyed");

            Debug.Assert(nameConstruction.Length > 0);
            Debug.Assert(destroyed > 0);
        }

        internal override bool CheckRequirement(Player p) => p.LairsDestroyed(construction) >= destroyed;

        internal override TextRequirement GetTextRequirement(Player p)
        {
            return new TextRequirement(CheckRequirement(p), $"Разрушить {construction.Name}: {p.LairsDestroyed(construction)}/{destroyed}");
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            construction = Config.FindConstruction(nameConstruction);
            nameConstruction = "";

            Debug.Assert(construction.Category == CategoryConstruction.Lair);
        }
    }

}
