using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static Fantasy_Kingdoms_Battle.XmlUtils;


namespace Fantasy_Kingdoms_Battle
{
    internal class ListDescriptorRequirements : List<DescriptorRequirement>
    {
        private bool linksTuned;
        public ListDescriptorRequirements(Descriptor forEntity, XmlNode n)
        {
            if (n != null)
            {
                string type;
                foreach (XmlNode r in n.SelectNodes("Requirement"))
                {
                    type = GetStringNotNull(r, "TypeRequirement");
                    if (type == "BuildedConstruction")
                        Add(new RequirementConstruction(forEntity, r));
                    else if (type == "DestroyedLairs")
                        Add(new RequirementDestroyedLairs(forEntity, r));
                    else if (type == "BuildedTypeConstruction")
                        Add(new RequirementTypeConstruction(forEntity, r));
                    else if (type == "GoodsInConstruction")
                        Add(new RequirementGoods(forEntity, r));
                    else if (type == "ExtensionInConstruction")
                        Add(new RequirementExtension(forEntity, r));
                    else
                        throw new Exception($"Неизвестный тип условия: {type}.");
                }
            }
        }

        internal DescriptorWithID ForEntity { get; }

        internal void TuneLinks()
        {
            Utils.Assert(!linksTuned);

            foreach (DescriptorRequirement r in this)
                r.TuneLinks();

            linksTuned = true;
        }
    }
}
