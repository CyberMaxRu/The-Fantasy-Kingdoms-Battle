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
    internal abstract class DescriptorProduct : DescriptorEntityForConstruction
    {
        public DescriptorProduct(DescriptorConstruction descriptor, XmlNode n) : base(descriptor, n)
        {
            Cost = GetInteger(n, "Cost");

            Debug.Assert(Cost >= 0);
            Debug.Assert(Cost <= 10_0000);
        }

        internal int Cost { get; }// Стоимость товара при покупке        
    }
}