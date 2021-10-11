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
    internal sealed class DescriptorCreatureTypeInterest : DescriptorEntity
    {
        public DescriptorCreatureTypeInterest(XmlNode n) : base(n)
        {
        }
    }
}
