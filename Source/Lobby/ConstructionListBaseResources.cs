using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionListBaseResources : List<ConstructionBaseResource>
    {
        public ConstructionListBaseResources(Construction c) : base(FormMain.Descriptors.BaseResources.Count)
        {
            foreach (DescriptorBaseResource br in FormMain.Descriptors.BaseResources)
                Add(new ConstructionBaseResource(c, br));
        }

        internal int Gold { get => this[0].Quantity; set { this[0].Quantity = value; } }
    }
}
