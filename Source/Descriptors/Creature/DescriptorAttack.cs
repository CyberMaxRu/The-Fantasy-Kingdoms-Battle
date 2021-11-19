using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum KindAttack { Melee, Range, Magic };
    internal class DescriptorAttack
    {
        public DescriptorAttack(XmlNode n)
        {
            ID = XmlUtils.GetStringNotNull(n, "ID");
            Name = XmlUtils.GetStringNotNull(n, "Name");
            KindAttack = (KindAttack)Enum.Parse(typeof(KindAttack), XmlUtils.GetStringNotNull(n, "KindAttack"));
            IsWeapon = XmlUtils.GetBooleanNotNull(n, "IsWeapon");

            foreach (DescriptorAttack ta in FormMain.Descriptors.TypeAttacks)
            {
                Debug.Assert(ta.ID != ID);
                Debug.Assert(ta.Name != Name);
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal KindAttack KindAttack { get; }
        internal bool IsWeapon { get; }
    }
}
