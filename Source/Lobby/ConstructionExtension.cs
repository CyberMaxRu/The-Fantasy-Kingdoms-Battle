using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionExtension : EntityForConstruction
    {
        public ConstructionExtension(Construction construction, DescriptorConstructionExtension descriptor) : base(construction, descriptor)
        {
            Descriptor = descriptor;
        }

        internal new DescriptorConstructionExtension Descriptor { get; }

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(Descriptor.Name, GetImageIndex());
            Program.formMain.formHint.AddStep5Description(Descriptor.Description);
            Program.formMain.formHint.AddStep9ListNeeds(Descriptor.ListNeeds, true);
            Program.formMain.formHint.AddStep9Interest(Descriptor.ModifyInterest, true);
        }
    }
}
