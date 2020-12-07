using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс информации об уровне здания
    internal sealed class Level
    {
        public Level(XmlNode n)
        {
            Pos = Convert.ToInt32(n.SelectSingleNode("Pos").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Builders = Convert.ToInt32(n.SelectSingleNode("Builders").InnerText);
            Income = n.SelectSingleNode("Income").InnerText != null ? Convert.ToInt32(n.SelectSingleNode("Income").InnerText) : 0;
            MaxHeroes = n.SelectSingleNode("MaxHeroes") != null ? Convert.ToInt32(n.SelectSingleNode("MaxHeroes").InnerText) : 0;

            Debug.Assert(Pos > 0);
            Debug.Assert(Cost >= 0);
            Debug.Assert(Income >= 0);

            // Загружаем требования
            XmlUtils.LoadRequirements(Requirements, n);
        }

        internal int Pos { get; }
        internal int Cost { get; }
        internal int Builders { get; }
        internal int Income { get; }
        internal int MinHeroes { get; }
        internal int MaxHeroes { get; }

        internal List<Requirement> Requirements = new List<Requirement>();
    }
}
