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
    // Класс монстров уровня логова
    internal sealed class MonsterLevelLair : Descriptor
    {
        private string idMonster;

        public MonsterLevelLair(XmlNode n) : base()
        {
            idMonster = n.SelectSingleNode("ID").InnerText;
            StartQuantity = GetInteger(n, "StartQuantity");
            MaxQuantity = GetInteger(n, "MaxQuantity");
            Level = GetInteger(n, "Level");
            DaysRespawn = GetInteger(n, "DaysRespawn");
            QuantityRespawn = GetInteger(n, "QuantityRespawn");

            Debug.Assert(idMonster.Length > 0);
            Debug.Assert(StartQuantity >= 0);
            Debug.Assert(MaxQuantity > 0);
            Debug.Assert(StartQuantity <= MaxQuantity);
            Debug.Assert(Level > 0);
            Debug.Assert(DaysRespawn >= 0);
            Debug.Assert(DaysRespawn <= 25);
            Debug.Assert(QuantityRespawn >= 0);
            //Debug.Assert(QuantityRespawn <= 49);
        }

        internal DescriptorCreature Monster { get; private set; }
        internal int StartQuantity { get; }
        internal int MaxQuantity { get; }
        internal int Level { get; }
        internal int DaysRespawn { get; }
        internal int QuantityRespawn { get; }
        internal List<Monster> Monsters { get; } = new List<Monster>();

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Monster = Config.FindCreature(idMonster);
            idMonster = "";

            Debug.Assert(Monster.CategoryCreature == CategoryCreature.Monster);
            Debug.Assert(Level <= Monster.MaxLevel);
        }
    }
}
