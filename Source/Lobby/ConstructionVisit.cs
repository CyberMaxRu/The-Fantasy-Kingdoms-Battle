using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class ConstructionVisit : EntityForConstruction
    {
        public ConstructionVisit(Construction construction, DescriptorEntityForActiveEntity descriptor) : base(construction, descriptor)
        {

        }
    }
}
