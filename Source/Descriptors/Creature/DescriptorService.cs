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
    internal sealed class DescriptorService : DescriptorEntityForCreature
    {
        public DescriptorService(XmlNode n) : base(n)
        {

        }

        internal override string GetTypeEntity() => "Услуга";
    }
}
