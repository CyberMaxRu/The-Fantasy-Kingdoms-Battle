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
    internal sealed class DescriptorProperty : DescriptorEntity
    {
        public DescriptorProperty(XmlNode n) : base(n)
        {
            Index = Descriptors.PropertiesCreature.Count;
        }

        internal int Index { get; }
    }
}