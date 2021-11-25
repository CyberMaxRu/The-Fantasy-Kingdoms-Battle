using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorConstructionImprovement : DescriptorEntityForActiveEntity
    {
        public DescriptorConstructionImprovement(DescriptorConstruction construction, XmlNode n) : base(construction, n)
        {
        }

        internal override string GetTypeEntity()
        {
            return "Улучшение";
        }
    }
}
