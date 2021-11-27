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
    // Класс настройки интереса для типа существ
    internal class DescriptorCreatureInterest : Descriptor
    {
        public DescriptorCreatureInterest(DescriptorInterest di, XmlNode n) : base()
        {
            Descriptor = di;

            MinValueOnHire = GetIntegerNotNull(n, "MinValueOnHire");
            MaxValueOnHire = GetIntegerNotNull(n, "MaxValueOnHire");

            Debug.Assert(MinValueOnHire >= 0);
            Debug.Assert(MinValueOnHire <= 1000);
            Debug.Assert(MaxValueOnHire >= 0);
            Debug.Assert(MaxValueOnHire <= 1000);
            Debug.Assert(MinValueOnHire < MaxValueOnHire);
        }

        internal DescriptorInterest Descriptor { get; }
        internal int MinValueOnHire { get; }// Минимальное значение при найме
        internal int MaxValueOnHire { get; }// Максимальное значение при найме
    }
}