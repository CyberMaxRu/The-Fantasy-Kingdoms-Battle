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
    internal sealed class DescriptorSpell : DescriptorSmallEntity
    {
        public DescriptorSpell(XmlNode n) : base(n)
        {
            Cooldown = GetIntegerNotNull(n, "Cooldown");
            Cost = new ListBaseResources(n.SelectSingleNode("Cost"));
        }

        internal int Cooldown { get; }
        internal ListBaseResources Cost { get; }

        internal override string GetTypeEntity()
        {
            return "Заклинание";
        }
    }
}
