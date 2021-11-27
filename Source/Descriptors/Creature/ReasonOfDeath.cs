using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorReasonOfDeath : DescriptorWithID
    {
        public DescriptorReasonOfDeath(XmlNode n) : base(n)
        {
        }
    }
}