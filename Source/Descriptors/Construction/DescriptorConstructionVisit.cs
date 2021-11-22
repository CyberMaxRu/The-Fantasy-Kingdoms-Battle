using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class DescriptorConstructionVisit : DescriptorEntityForConstruction
    {
        public DescriptorConstructionVisit(DescriptorConstruction construction, XmlNode n) : base(construction, n)
        {

        }
    }
}
