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

    internal sealed class DescriptorMissionQuest : DescriptorWithID
    {
        public DescriptorMissionQuest(XmlNode n) : base(n)
        {
            TypeQuest = (TypeQuest)Enum.Parse(typeof(TypeQuest), GetStringNotNull(n, "TypeQuest"));
            From = GetStringNotNull(n, "From");
            Description = GetStringNotNull(n, "Description");
            Turn = GetIntegerNotNull(n, "Turn");
        }
        
        internal TypeQuest TypeQuest { get; }
        internal string From { get; }// От кого поступил квест
        internal string Description { get; }
        internal int Turn { get; }// Ход, на котором выдается квест
    }
}
