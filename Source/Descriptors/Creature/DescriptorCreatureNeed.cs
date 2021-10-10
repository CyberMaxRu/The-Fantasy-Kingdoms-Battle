using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс настройки потребности для типа существ
    internal sealed class DescriptorCreatureNeed : Descriptor
    {
        public DescriptorCreatureNeed(DescriptorNeed nd, XmlNode n) : base()
        {
            Descriptor = nd;

            MinValueOnHire = GetIntegerNotNull(n, "MinValueOnHire");
            MaxValueOnHire = GetIntegerNotNull(n, "MaxValueOnHire");
            IncreasePerDay = GetIntegerNotNull(n, "IncreasePerDay");

            Debug.Assert(MinValueOnHire >= 0);
            Debug.Assert(MinValueOnHire <= 100);
            Debug.Assert(MaxValueOnHire >= 0);
            Debug.Assert(MaxValueOnHire <= 100);
            Debug.Assert(MinValueOnHire < MaxValueOnHire);
            Debug.Assert(IncreasePerDay > 0);
            Debug.Assert(IncreasePerDay < 100);
        }

        internal DescriptorNeed Descriptor { get; }
        internal int MinValueOnHire { get; }// Минимальное значение при найме
        internal int MaxValueOnHire { get; }// Максимальное значение при найме
        internal int IncreasePerDay { get; }// Увеличение в день
    }
}
