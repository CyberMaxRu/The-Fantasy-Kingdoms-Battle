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
    sealed internal class Mission
    {
        public Mission(string filename)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(filename);
            XmlNode n = xml.SelectSingleNode("Mission");

            ID = GetStringNotNull(n, "ID");
            Name = GetStringNotNull(n, "Name");
            Description = GetStringNotNull(n, "Description");


        }

        internal string ID { get; }// Уникальный идентификатор миссии
        internal string Name { get; }
        internal string Description { get; }

        internal List<MissionPlayer> Players { get; } = new List<MissionPlayer>();
        internal List<MissionMessage> Messages { get; } = new List<MissionMessage>();
        internal List<MissionQuest> Quests { get; } = new List<MissionQuest>();
    }
}
