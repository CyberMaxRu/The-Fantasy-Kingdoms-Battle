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
    internal enum NamePropertyCreature { Honor = 0, Enthusiasm = 1, Morale = 2, Luck = 3 };

    internal sealed class DescriptorPropertyCreature : DescriptorEntity
    {
        public DescriptorPropertyCreature(XmlNode n) : base(n)
        {
            Index = Config.PropertiesCreature.Count;
            NameType = GetStringNotNull(n, "NameType");
            MinValue = GetIntegerNotNull(n, "MinValue");
            MaxValue = GetIntegerNotNull(n, "MaxValue");

            NameProperty = (NamePropertyCreature)Enum.Parse(typeof(NamePropertyCreature), ID);

            Debug.Assert(MinValue >= -100);
            Debug.Assert(MinValue <= 0);
            Debug.Assert(MaxValue > 0);
            Debug.Assert(MaxValue <= 100);
            Debug.Assert(MinValue < MaxValue);
        }

        internal int Index { get; }
        internal NamePropertyCreature NameProperty { get; }
        internal string NameType { get; }
        internal int MinValue { get; }
        internal int MaxValue { get; }
    }
}
