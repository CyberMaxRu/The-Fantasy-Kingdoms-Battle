using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorMissionMessage
    {
        public DescriptorMissionMessage(XmlNode n)
        {
            Turn = GetIntegerNotNull(n, "Turn");

            // Загружаем части сообщения
            foreach (XmlNode np in n.SelectNodes("Part"))
            {
                Parts.Add(new DescriptorMissionMessagePart(np));
            }
        }

        internal int Turn { get; }
        internal List<DescriptorMissionMessagePart> Parts { get; } = new List<DescriptorMissionMessagePart>();
    }

    internal sealed class DescriptorMissionMessagePart
    {
        public DescriptorMissionMessagePart(XmlNode n)
        {
            From = GetStringNotNull(n, "From");
            Text = GetStringNotNull(n, "Text");
        }

        internal string From { get; }
        internal string Text { get; }
    }
}
