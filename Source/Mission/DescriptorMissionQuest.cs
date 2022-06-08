using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypeQuest { Primary, Secondary };

    internal sealed class DescriptorMissionQuest
    {
        public DescriptorMissionQuest(XmlNode n)
        {
            ID = GetStringNotNull(n, "ID");
            TypeQuest = (TypeQuest)Enum.Parse(typeof(TypeQuest), GetStringNotNull(n, "TypeQuest"));
            Name = GetStringNotNull(n, "Name");
            Description = GetStringNotNull(n, "Description");
        }
        
        internal string ID { get; }
        internal TypeQuest TypeQuest { get; }
        internal string Name { get; }
        internal string Description { get; }
    }
}
