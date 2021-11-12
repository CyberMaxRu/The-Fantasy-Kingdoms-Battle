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
        public ListDescriptorRequirements(DescriptorCellMenu forCellMenu, XmlNode n)
        {
            if (n != null)
            {
                string type;
                foreach (XmlNode r in n.SelectNodes("Requirement"))
                {
                    type = GetStringNotNull(r, "TypeRequirement");
                    if (type == "BuildedConstruction")
                        Add(new RequirementConstruction(forCellMenu, r));
                    else if (type == "DestroyedLairs")
                        Add(new RequirementDestroyedLairs(forCellMenu, r));
                    else if (type == "BuildedTypeConstruction")
                        Add(new RequirementTypeConstruction(forCellMenu, r));
                    else if (type == "GoodsInConstruction")
                        Add(new RequirementGoods(forCellMenu, r));
                    else if (type == "ExtensionInConstruction")
                        Add(new RequirementExtension(forCellMenu, r));
                    else
                        throw new Exception($"Неизвестный тип условия: {type}.");
                }
            }
        }

        internal DescriptorCellMenu CellMenu { get; }

        internal void TuneLinks()
        {
            foreach (DescriptorRequirement r in this)
                r.TuneLinks();
        }
    }
}
