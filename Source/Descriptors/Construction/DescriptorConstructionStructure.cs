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
            Durability = GetIntegerNotNull(n, "Durability", ID, 0, 1_000_000);
        }

        internal int Durability { get; }// Прочность сооружения
    }
}
