using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс требования
    internal sealed class Requirement
    {
        public Requirement(Building b, int l)
        {
            Debug.Assert(l >= 0);
            Debug.Assert(l <= b.MaxLevel);

            Building = b;
            Level = l;
        }

        internal Building Building { get; }
        internal int Level { get; }
    }

    // Класс информации об уровне здания
    internal sealed class Level
    {
        public Level(XmlNode n)
        {
            Pos = Convert.ToInt32(n.SelectSingleNode("Pos").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Income = n.SelectSingleNode("Income").InnerText != null ? Convert.ToInt32(n.SelectSingleNode("Income").InnerText) : 0;

            Debug.Assert(Pos > 0);
            Debug.Assert(Cost >= 0);
            Debug.Assert(Income >= 0);

            // Загружаем требования
            XmlNode nr = n.SelectSingleNode("Requirements");
            if (nr != null)
            {
                Level level;

                foreach (XmlNode r in nr.SelectNodes("Requirement"))
                {
                    Requirements.Add(new Requirement(
                        FormMain.Config.FindBuilding(r.SelectSingleNode("Building").InnerText),
                        Convert.ToInt32(r.SelectSingleNode("Level").InnerText)));
                }
            }
        }

        internal int Pos { get; }
        internal int Cost { get; }
        internal int Income { get; }

        internal List<Requirement> Requirements = new List<Requirement>();
    }
}
