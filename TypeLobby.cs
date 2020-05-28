using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс типа (конфигурации) лобби
    internal sealed class TypeLobby
    {
        public TypeLobby(XmlNode n)
        {
            Name = n.SelectSingleNode("Name").InnerText;
            QuantityPlayers = Convert.ToInt32(n.SelectSingleNode("QuantityPlayers").InnerText);
            DurabilityCastle = Convert.ToInt32(n.SelectSingleNode("DurabilityCastle").InnerText);
            Gold = Convert.ToInt32(n.SelectSingleNode("Gold").InnerText);
            MaxHeroes = Convert.ToInt32(n.SelectSingleNode("MaxHeroes").InnerText);

            Debug.Assert(QuantityPlayers >= 2);
            Debug.Assert(QuantityPlayers >= 8);
            Debug.Assert(QuantityPlayers % 2 == 0);
            Debug.Assert(DurabilityCastle > 0);
            Debug.Assert(DurabilityCastle <= 1000);
            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= 100000);
            Debug.Assert(MaxHeroes >= 1);
            Debug.Assert(MaxHeroes <= 100);// Здесь проверять через максим. число героев на поле боя
        }

        internal string Name { get; }
        internal int QuantityPlayers { get; }
        internal int DurabilityCastle { get; }
        internal int Gold { get; }
        internal int MaxHeroes { get; }
    }
}
