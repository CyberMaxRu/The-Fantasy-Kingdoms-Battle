using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    sealed internal class DescriptorTimeOfDay : DescriptorVisual
    {
        public DescriptorTimeOfDay(XmlNode n): base(n)
        {
            Index = Descriptors.TimesOfDay.Count;
        }

        internal int Index { get; }
    }
}
