using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс информации об уровне здания
    internal sealed class Level
    {
        public Level(XmlNode n)
        {
            Pos = Convert.ToInt32(n.SelectSingleNode("Pos").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Builders = XmlUtils.GetIntegerNotNull(n.SelectSingleNode("Builders"));
            PointConstructionTemple = XmlUtils.GetInteger(n.SelectSingleNode("PointConstructionTemple"));
            PointConstructionTradePost = XmlUtils.GetInteger(n.SelectSingleNode("PointConstructionTradePost"));
            Income = n.SelectSingleNode("Income").InnerText != null ? Convert.ToInt32(n.SelectSingleNode("Income").InnerText) : 0;
            MaxHeroes = n.SelectSingleNode("MaxHeroes") != null ? Convert.ToInt32(n.SelectSingleNode("MaxHeroes").InnerText) : 0;
            GreatnessByConstruction = XmlUtils.GetInteger(n.SelectSingleNode("GreatnessByConstruction"));
            GreatnessPerDay = XmlUtils.GetInteger(n.SelectSingleNode("GreatnessPerDay"));

            Debug.Assert(Pos > 0);
            Debug.Assert(Cost >= 0);
            Debug.Assert(Builders >= 0);
            Debug.Assert(Builders <= 5);
            Debug.Assert(PointConstructionTemple >= 0);
            Debug.Assert(PointConstructionTemple <= 1);
            Debug.Assert(PointConstructionTradePost >= 0);
            Debug.Assert(PointConstructionTradePost <= 1);
            Debug.Assert(Income >= 0);
            Debug.Assert(GreatnessByConstruction >= 0);
            Debug.Assert(GreatnessPerDay >= 0);

            if ((Builders > 0) || (PointConstructionTemple > 0))
            {
                Debug.Assert(Cost > 0);
            }

            // Загружаем требования
            XmlUtils.LoadRequirements(Requirements, n);
        }

        internal int Pos { get; }
        internal int Cost { get; }
        internal int Builders { get; }
        internal int PointConstructionTemple { get; }
        internal int PointConstructionTradePost { get; }
        internal int Income { get; }
        internal int GreatnessByConstruction { get; }// Дает очков Величия при постройке
        internal int GreatnessPerDay { get; }// Дает очков Величия в день
        internal int MinHeroes { get; }
        internal int MaxHeroes { get; }

        internal List<Requirement> Requirements = new List<Requirement>();
    }
}
