using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс заклинания
    internal sealed class DescriptorConstructionSpell : DescriptorEntityForActiveEntity
    {
        public DescriptorConstructionSpell(DescriptorConstruction construction, XmlNode n) : base(construction, n)
        {
        }

        internal override string GetTypeEntity()
        {
            return "Заклинание";
        }
    }
}
