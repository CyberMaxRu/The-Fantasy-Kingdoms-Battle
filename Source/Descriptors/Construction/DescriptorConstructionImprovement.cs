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
    internal sealed class DescriptorConstructionImprovement : DescriptorEntityForConstruction
    {
        public DescriptorConstructionImprovement(DescriptorConstruction construction, XmlNode n) : base(construction, n)
        {
            foreach (DescriptorConstructionImprovement ce in Construction.Improvements)
            {
                Debug.Assert(ce.ID != ID);
                Debug.Assert(ce.Name != Name);
                Debug.Assert(ce.ImageIndex != ImageIndex);
            }
        }

        internal override string GetTypeEntity()
        {
            return "Улучшение";
        }
    }
}
