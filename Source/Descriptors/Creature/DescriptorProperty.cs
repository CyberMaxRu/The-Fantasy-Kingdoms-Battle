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
    internal enum NamePropertyCreature { Honor = 0, Enthusiasm = 1, Morale = 2, Luck = 3, Stealth = 4, Vigilance = 5 };

    internal sealed class DescriptorProperty : DescriptorEntity
    {
        public DescriptorProperty(XmlNode n) : base(n)
        {
            Index = Descriptors.PropertiesCreature.Count;
            MinValue = GetIntegerNotNull(n, "MinValue");
            MaxValue = GetIntegerNotNull(n, "MaxValue");

            NameProperty = (NamePropertyCreature)Enum.Parse(typeof(NamePropertyCreature), ID);

            Debug.Assert(MinValue >= -1000);
            Debug.Assert(MinValue <= 0);
            Debug.Assert(MaxValue > 0);
            Debug.Assert(MaxValue <= 1000);
            Debug.Assert(MinValue < MaxValue);
        }

        internal int Index { get; }
        internal NamePropertyCreature NameProperty { get; }
        internal int MinValue { get; }
        internal int MaxValue { get; }
    }
}