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
    internal sealed class RequirementExtension : DescriptorRequirement
    {
        private DescriptorConstruction Construction;
        private string nameConstruction;
        private DescriptorConstructionExtension Extension;
        private string nameExtension;

        public RequirementExtension(DescriptorCellMenu forCellMenu, XmlNode n) : base(forCellMenu, n)
        {
            nameConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            nameExtension = XmlUtils.GetStringNotNull(n, "Extension");

            Debug.Assert(nameConstruction.Length > 0);
            Debug.Assert(nameExtension.Length > 0);
        }

        internal override bool CheckRequirement(Player p)
        {
            return p.FindConstruction(Construction.ID).ExtensionAvailabled(Extension);
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Construction = Config.FindConstruction(nameConstruction);
            Extension = Construction.FindExtension(nameExtension, true);
            nameConstruction = "";
            nameExtension = "";

            bool founded = false;
            foreach (DescriptorCellMenuForConstruction cm in Construction.ListResearches)
                if (cm.NameEntity == Extension.ID)
                {
                    //cm.UseForResearches.Add(Goods);
                    founded = true;
                    break;
                }

            if (ForCellMenu is DescriptorCellMenuForConstruction cmc)
                Debug.Assert(Extension.ID != cmc.NameEntity, $"Расширение {Extension.ID} требует само себя.");
            Debug.Assert(founded, $"Расширение {Extension.ID} не найдено в {Construction.ID}.");

            Extension.UseForResearch.Add(ForCellMenu);
        }

        internal override TextRequirement GetTextRequirement(Player p)
        {
            return new TextRequirement(CheckRequirement(p), $"{Extension.Name} ({Construction.Name})");
        }
    }
}