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
    internal class DescriptorEntityForConstruction : DescriptorSmallEntity
    {
        public DescriptorEntityForConstruction(DescriptorConstruction descriptor, XmlNode n) : base(n)
        {
            Descriptor = descriptor;
        }

        internal DescriptorConstruction Descriptor { get; private set; }
    }
}
