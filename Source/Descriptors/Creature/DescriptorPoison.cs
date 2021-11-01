using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal class DescriptorPoison : DescriptorWithID
    {
        public DescriptorPoison(XmlNode n) : base(n)
        {

            foreach (DescriptorAttack ta in FormMain.Config.TypeAttacks)
            {
                Debug.Assert(ta.ID != ID);
                Debug.Assert(ta.Name != Name);
            }
        }

    }
}
