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
    // Класс типы награды
    internal sealed class DescriptorReward : Descriptor
    {
        public DescriptorReward(XmlNode n) : base()
        {
            Cost = new ListBaseResources(GetInteger(n, "Gold"));
            Greatness = GetInteger(n, "Greatness");

            Debug.Assert(Cost.Gold >= 0);
            Debug.Assert(Cost.Gold <= 50_000);
            Debug.Assert(Greatness >= 0);
            Debug.Assert(Greatness <= 10_000);
        }

        internal ListBaseResources Cost { get; }
        internal int Greatness { get; }
    }
}
