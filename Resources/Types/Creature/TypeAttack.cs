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
    internal class TypeAttack
    {
        public TypeAttack(XmlNode n)
        {
            ID = XmlUtils.GetStringNotNull(n.SelectSingleNode("ID"));
            Name = XmlUtils.GetStringNotNull(n.SelectSingleNode("Name"));
            KindAttack = (KindAttack)Enum.Parse(typeof(KindAttack), n.SelectSingleNode("KindAttack").InnerText);
            IsWeapon = XmlUtils.GetBoolNotNull(n.SelectSingleNode("IsWeapon"));

            foreach (TypeAttack ta in FormMain.Config.TypeAttacks)
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
