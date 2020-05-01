using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Класс информации об уровне здания
    internal sealed class Level
    {
        public Level(XmlNode n)
        {
            Pos = Convert.ToInt32(n.SelectSingleNode("Pos").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Income = n.SelectSingleNode("Income").InnerText != null ? Convert.ToInt32(n.SelectSingleNode("Income").InnerText) : 0;
        }

        internal int Pos { get; }
        internal int Cost { get; }
        internal int Income { get; }
    }
}
