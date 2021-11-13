using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorResource : DescriptorSmallEntity
    {
        public DescriptorResource(XmlNode n) : base(n)
        {
            foreach (DescriptorResource dr in Descriptors.Resources)
            {
                Debug.Assert(dr.ID != ID);
                Debug.Assert(dr.Name != Name);
                //Debug.Assert(cv.Description != Description);
                //Debug.Assert(cv.ImageIndex != ImageIndex);
            }
        }
    }
}
