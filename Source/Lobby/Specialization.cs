﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class Specialization : SmallEntity
    {
        public Specialization(Creature c, DescriptorSpecialization type) : base()
        {
            Creature = c;
            TypeSpecialization = type;
        }

        internal Creature Creature { get; }
        internal DescriptorSpecialization TypeSpecialization { get; }

        internal override string GetCost()
        {
            return "";
        }

        internal override int GetImageIndex()
        {
            return TypeSpecialization.ImageIndex;
        }

        internal override int GetLevel()
        {
            return 0;
        }

        internal override bool GetNormalImage()
        {
            return true;
        }

        internal override int GetQuantity()
        {
            return 0;
        }

        internal override void PrepareHint()
        {
                        
        }
    }
}