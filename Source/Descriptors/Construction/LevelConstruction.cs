using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс информации об уровне здания
    internal sealed class LevelConstruction
    {
        public LevelConstruction(XmlNode n)
        {
            Pos = GetIntegerNotNull(n, "Pos");
            Cost = GetIntegerNotNull(n, "Cost");
            Builders = GetIntegerNotNull(n, "Builders");
            Income = GetInteger(n, "Income");
            MaxHeroes = GetInteger(n, "MaxHeroes");

            Debug.Assert(Pos > 0);
            Debug.Assert(Cost >= 0);
            Debug.Assert(Builders >= 0);
            Debug.Assert(Builders <= 5);
            Debug.Assert(Income >= 0);

            if (Builders > 0)
            {
                //Debug.Assert(Cost > 0);
            }

            // Загружаем требования
            LoadRequirements(Requirements, n);
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
