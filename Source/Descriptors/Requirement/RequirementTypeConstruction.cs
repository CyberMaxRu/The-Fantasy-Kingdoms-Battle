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
    internal sealed class RequirementTypeConstruction : DescriptorRequirement
    {
        private DescriptorTypeConstruction typeConstruction;
        private string nameTypeConstruction;
        private int quantity;

        public RequirementTypeConstruction(Descriptor forEntity, XmlNode n, ListDescriptorRequirements list) : base(forEntity, n, list)
        {
            nameTypeConstruction = XmlUtils.GetStringNotNull(n, "TypeConstruction");
            quantity = XmlUtils.GetInteger(n, "Quantity");

            Debug.Assert(nameTypeConstruction.Length > 0);
            Debug.Assert(quantity > 0);
        }

        internal override bool CheckRequirement(Player p) => base.CheckRequirement(p) || (p.TypeConstructionBuilded(typeConstruction) >= quantity);
        
        internal override void TuneLinks()
        {
            base.TuneLinks();

            typeConstruction = Descriptors.FindTypeConstruction(nameTypeConstruction);
            nameTypeConstruction = "";
        }

        internal override (bool, string) GetTextRequirement(Player p, Construction inConstruction = null) => (CheckRequirement(p), $"Сооружение с типом \"{typeConstruction.Name}\": {quantity} шт.");
    }
}
