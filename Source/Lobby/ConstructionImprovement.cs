﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionImprovement : EntityForConstruction
    {
        public ConstructionImprovement(Construction construction, DescriptorConstructionImprovement descriptor) : base(construction, descriptor)
        {
            Descriptor = descriptor;
        }

        internal new DescriptorConstructionImprovement Descriptor { get; }

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(Descriptor.Name, GetImageIndex());
            Program.formMain.formHint.AddStep5Description(Descriptor.Description);
        }
    }
}