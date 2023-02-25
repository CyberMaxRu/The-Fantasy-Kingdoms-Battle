using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class DescriptorConstructionStructure : DescriptorEntityForActiveEntity
    {
        public DescriptorConstructionStructure(DescriptorConstruction forConstruction, XmlNode n) : base(forConstruction, n)
        {
            Durability = new Integer1000(GetIntegerNotNull(n, "Durability", ID, 0, 1_000_000));
        }

        internal Integer1000 Durability { get; }// Прочность сооружения
    }
}
