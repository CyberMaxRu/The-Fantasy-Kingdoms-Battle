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
        public DescriptorMissionMessage(XmlNode n, DescriptorMission dm)
        {
            Turn = GetIntegerNotNull(n, "Turn");

            // Загружаем части сообщения
            foreach (XmlNode np in n.SelectNodes("Part"))
            {
                Parts.Add(new DescriptorMissionMessagePart(np, dm));
            }
        }

        internal int Turn { get; }
        internal List<DescriptorMissionMessagePart> Parts { get; } = new List<DescriptorMissionMessagePart>();
    }

    internal sealed class DescriptorMissionMessagePart
    {
        public DescriptorMissionMessagePart(XmlNode n, DescriptorMission dm)
        {
            From = dm.FindMember(GetStringNotNull(n, "From"));
            Text = GetDescription(n, "Text");
            foreach (DescriptorMissionPlayer dmp in dm.Players)
                Text = Text.Replace($"#{dmp.ID}#", FormMain.Descriptors.HumanPlayers[0].Name);
        }

        internal DescriptorMissionMember From { get; }
        internal string Text { get; }
    }
}
