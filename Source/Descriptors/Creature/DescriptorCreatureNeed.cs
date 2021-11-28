using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс настройки потребности для типа существ
    internal sealed class DescriptorCreatureNeed : DescriptorCreaturePropertyMain
    {
        public DescriptorCreatureNeed(DescriptorNeed nd, XmlNode n) : base(n)
        {
            Descriptor = nd;

            IncreasePerDay = GetIntegerNotNull(n, "IncreasePerDay");

            Debug.Assert(MinValueOnHire >= 0);
            Debug.Assert(MinValueOnHire <= 1000);
            Debug.Assert(MaxValueOnHire >= 0);
            Debug.Assert(MaxValueOnHire <= 1000);
            Debug.Assert(MinValueOnHire < MaxValueOnHire);
            Debug.Assert(IncreasePerDay > 0);
            Debug.Assert(IncreasePerDay < 1000);
        }

        internal DescriptorNeed Descriptor { get; }
        internal int IncreasePerDay { get; }// Увеличение в день
    }
}