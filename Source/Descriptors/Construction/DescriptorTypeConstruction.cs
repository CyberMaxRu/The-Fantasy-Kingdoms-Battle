using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorTypeConstruction : DescriptorWithID
    {
        public DescriptorTypeConstruction(XmlNode n) : base(n)
        {
            foreach (DescriptorTypeConstruction tc in Config.TypeConstructions)
            {
                Debug.Assert(tc.ID != ID);
                Debug.Assert(tc.Name != Name);
            }
        }
    }
}
