using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Класс типа (конфигурации) лобби
    internal sealed class TypeLobby
    {
        public TypeLobby(XmlNode n)
        {
            Name = n.SelectSingleNode("Name").InnerText;
            Players = Convert.ToInt32(n.SelectSingleNode("Players").InnerText);
        }

        internal string Name { get; }
        internal int Players { get; }
    }
}
