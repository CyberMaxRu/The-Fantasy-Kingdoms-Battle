using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal class DescriptorVisitorEvent : DescriptorEntityForCreature
    {
        public DescriptorVisitorEvent(string id, string name, string description, int imageIndex) : base(id, name, description, imageIndex)
        {

        }

        internal override string GetTypeEntity() => "Мероприятие";
    }
}
