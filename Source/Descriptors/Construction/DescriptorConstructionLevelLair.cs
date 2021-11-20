using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс монстров уровня логова
    internal sealed class DescriptorConstructionLevelLair : Descriptor
    {
        private string idMonster;

        public DescriptorConstructionLevelLair(XmlNode n) : base()
        {
            idMonster = GetStringNotNull(n, "ID");
            StartQuantity = GetInteger(n, "StartQuantity");
            MaxQuantity = GetInteger(n, "MaxQuantity");
            Level = GetInteger(n, "Level");
            DaysRespawn = GetInteger(n, "DaysRespawn");
            QuantityInRespawn = GetInteger(n, "QuantityRespawn");

            Debug.Assert(idMonster.Length > 0);
            Debug.Assert(StartQuantity >= 0);
            Debug.Assert(MaxQuantity > 0);
            Debug.Assert(StartQuantity <= MaxQuantity);
            Debug.Assert(Level > 0);
            Debug.Assert(DaysRespawn >= 0);
            Debug.Assert(DaysRespawn <= 25);
            Debug.Assert(QuantityInRespawn >= 0);
            //Debug.Assert(QuantityRespawn <= 49);
        }

        internal DescriptorCreature Monster { get; private set; }
        internal int StartQuantity { get; }
        internal int MaxQuantity { get; }
        internal int Level { get; }
        internal int DaysRespawn { get; }
        internal int QuantityInRespawn { get; }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Monster = Descriptors.FindCreature(idMonster);
            idMonster = "";

            Debug.Assert(Monster.CategoryCreature == CategoryCreature.Monster);
            Debug.Assert(Level <= Monster.MaxLevel);
        }
    }
}
