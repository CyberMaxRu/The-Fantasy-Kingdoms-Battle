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
            Builders = XmlUtils.GetIntegerNotNull(n, "Builders");
            Income = XmlUtils.GetInteger(n, "Income");
            GreatnessByConstruction = XmlUtils.GetInteger(n, "GreatnessByConstruction");
            GreatnessPerDay = XmlUtils.GetInteger(n, "GreatnessPerDay");
            BuildersPerDay = XmlUtils.GetInteger(n, "BuildersPerDay");
            MaxHeroes = n.SelectSingleNode("MaxHeroes") != null ? Convert.ToInt32(n.SelectSingleNode("MaxHeroes").InnerText) : 0;

            Debug.Assert(Pos > 0);
            Debug.Assert(Cost >= 0);
            Debug.Assert(Builders >= 0);
            Debug.Assert(Builders <= 5);
            Debug.Assert(Income >= 0);
            Debug.Assert(GreatnessByConstruction >= 0);
            Debug.Assert(GreatnessPerDay >= 0);
            Debug.Assert(BuildersPerDay >= 0);

            if (Builders > 0)
            {
                //Debug.Assert(Cost > 0);
            }

            // Загружаем требования
            XmlUtils.LoadRequirements(Requirements, n);
        }

        internal int Pos { get; }
        internal int Cost { get; }
        internal int Builders { get; }
        internal int Income { get; }
        internal int GreatnessByConstruction { get; }// Дает очков Величия при постройке
        internal int GreatnessPerDay { get; }// Дает очков Величия в день
        internal int BuildersPerDay { get; }// Дает строителей в день
        internal int MinHeroes { get; }
        internal int MaxHeroes { get; }

        internal List<Requirement> Requirements = new List<Requirement>();
    }
}
