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

            // Загружаем участников
            XmlNode nmb = n.SelectSingleNode("Members");
            foreach (XmlNode npmb in nmb.SelectNodes("Member"))
            {
                Members.Add(new DescriptorMissionMember(npmb));
            }

            // Загружаем игроков
            XmlNode np = n.SelectSingleNode("Players");
            foreach (XmlNode npl in np.SelectNodes("Player"))
            {
                Players.Add(new DescriptorMissionPlayer(npl, Players.Count));
            }

            // Загружаем сообщения
            XmlNode nm = n.SelectSingleNode("Messages");
            foreach (XmlNode nml in nm.SelectNodes("Message"))
            {
                Messages.Add(new DescriptorMissionMessage(nml, this));
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
        internal List<DescriptorMissionMember> Members { get; } = new List<DescriptorMissionMember>();
        internal List<DescriptorMissionMessage> Messages { get; } = new List<DescriptorMissionMessage>();
        internal List<DescriptorMissionQuest> Quests { get; } = new List<DescriptorMissionQuest>();

        internal DescriptorMissionMember FindMember(string id)
        {
            foreach (DescriptorMissionMember m in Members)
            {
                if (m.ID == id)
                    return m;
            }

            throw new Exception($"Участник миссии {id} не найден.");
        }

        internal DescriptorMissionQuest FindQuest(string id)
        {
            foreach (DescriptorMissionQuest q in Quests)
            {
                if (q.ID == id)
                    return q;
            }

            throw new Exception($"Квест {id} не найден.");
        }
    }
}
