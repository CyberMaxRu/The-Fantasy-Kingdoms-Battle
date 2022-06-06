using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle.Source.Mission
{
    // Класс одиночной миссии
    sealed internal class Mission
    {
        public Mission(XmlNode n)
        {
            ID = GetStringNotNull(n, "ID");
            Name = GetStringNotNull(n, "Name");
            Description = GetStringNotNull(n, "Description");
        }

        internal string ID { get; }// Уникальный идентификатор миссии
        internal string Name { get; }
        internal string Description { get; }
    }
}
