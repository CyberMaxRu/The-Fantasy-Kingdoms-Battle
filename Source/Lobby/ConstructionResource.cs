using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionResource : EntityForConstruction
    {
        public ConstructionResource(Construction construction, DescriptorResource descriptor) : base(construction, descriptor)
        {
            DescriptorResource = descriptor;
        }

        internal DescriptorResource DescriptorResource { get; }

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(Descriptor.Name, GetImageIndex());
            Program.formMain.formHint.AddStep3Type("Ресурс");
            Program.formMain.formHint.AddStep5Description(Descriptor.Description);
        }
    }
}
