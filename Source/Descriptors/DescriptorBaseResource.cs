﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorBaseResource : DescriptorEntity
    {
        public DescriptorBaseResource(XmlNode n) : base(n)
        {
            ImageIndex16 = XmlUtils.GetIntegerNotNull(n, "ImageIndex16");
            Number = Config.BaseResources.Count;

            foreach (DescriptorBaseResource br in Config.BaseResources)
            {
                Debug.Assert(br.ID != ID);
                Debug.Assert(br.Name != Name);
                Debug.Assert(br.ImageIndex != ImageIndex);
            }
        }

        internal int Number { get; }
        internal int ImageIndex16 { get; }
    }
}