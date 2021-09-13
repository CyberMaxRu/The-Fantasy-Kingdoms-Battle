using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorTypeConstruction : Descriptor
    {
        public DescriptorTypeConstruction(XmlNode n) : base()
        {
            ID = XmlUtils.GetStringNotNull(n, "ID");
            Name = XmlUtils.GetStringNotNull(n, "Name");

            foreach (DescriptorTypeConstruction tc in Config.TypeConstructions)
            {
                Debug.Assert(tc.ID != ID);
                Debug.Assert(tc.Name != Name);
            }
        }

        internal string ID { get; }
        internal string Name { get; }
    }
}
