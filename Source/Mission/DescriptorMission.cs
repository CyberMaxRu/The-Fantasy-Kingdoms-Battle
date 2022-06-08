using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс одиночной миссии
    sealed internal class DescriptorMission
    {
        public DescriptorMission(string filename)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(filename);
            XmlNode n = xml.SelectSingleNode("Mission");

            ID = GetStringNotNull(n, "ID");
            Name = GetStringNotNull(n, "Name");
            Description = GetStringNotNull(n, "Description");

            // Загружаем игроков
            XmlNode np = n.SelectSingleNode("Players");
            foreach (XmlNode npl in np.SelectNodes("Player"))
            {
                Players.Add(new DescriptorMissionPlayer(npl));
            }

            // Загружаем сообщения
            XmlNode nm = n.SelectSingleNode("Messages");
            foreach (XmlNode nml in nm.SelectNodes("Message"))
            {
                Messages.Add(new DescriptorMissionMessage(nml));
            }

            // Загружаем задания
            XmlNode nq = n.SelectSingleNode("Quests");
            foreach (XmlNode nql in nq.SelectNodes("Quest"))
            {
                Quests.Add(new DescriptorMissionQuest(nql));
            }
        }

        internal string ID { get; }// Уникальный идентификатор миссии
        internal string Name { get; }
        internal string Description { get; }

        internal List<DescriptorMissionPlayer> Players { get; } = new List<DescriptorMissionPlayer>();
        internal List<DescriptorMissionMessage> Messages { get; } = new List<DescriptorMissionMessage>();
        internal List<DescriptorMissionQuest> Quests { get; } = new List<DescriptorMissionQuest>();
    }
}
