﻿using System;
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

        public RequirementExtension(Descriptor forEntity, XmlNode n, ListDescriptorRequirements list) : base(forEntity, n, list)
        {
            nameConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            nameExtension = XmlUtils.GetStringNotNull(n, "Extension");

            Debug.Assert(nameConstruction.Length > 0);
            Debug.Assert(nameExtension.Length > 0);
        }

        internal override bool CheckRequirement(Player p) => base.CheckRequirement(p) || p.FindConstruction(Construction.ID).ExtensionAvailabled(Extension);

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Construction = Descriptors.FindConstruction(nameConstruction);
            Extension = Construction.FindEntity(nameExtension) as DescriptorConstructionExtension;
            Utils.Assert(Extension != null);
            nameConstruction = "";
            nameExtension = "";

            bool founded = false;
            foreach (DescriptorActionForEntity cm in Construction.CellsMenu)
                if (cm.IDCreatedEntity == Extension.ID)
                {
                    //cm.UseForResearches.Add(Goods);
                    founded = true;
                    break;
                }

            if (ForEntity is DescriptorConstructionExtension dce)
                Debug.Assert(Extension.ID != dce.ID, $"Расширение {Extension.ID} требует само себя.");
            Debug.Assert(founded, $"Расширение {Extension.ID} не найдено в {Construction.ID}.");

            Extension.UseForResearch.Add(ForEntity as DescriptorSmallEntity);
        }

        internal override (bool, string) GetTextRequirement(Player p, Construction inConstruction = null)
        {
            return (CheckRequirement(p), $"{Extension.Name} ({Construction.Name})");
        }
    }
}