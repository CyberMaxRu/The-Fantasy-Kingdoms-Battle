using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class TypeAttack
    {
        public TypeAttack(XmlNode n)
        {
            ID = XmlUtils.GetStringNotNull(n.SelectSingleNode("ID"));
            Name = XmlUtils.GetStringNotNull(n.SelectSingleNode("Name"));
        }

        internal string ID { get; }
        internal string Name { get; }
    }

    internal sealed class TypeAttackMelee : TypeAttack
    {
        public TypeAttackMelee(XmlNode n) : base(n)
        {
            IsWeapon = XmlUtils.GetBoolNotNull(n.SelectSingleNode("IsWeapon"));

            foreach (TypeAttackMelee ta in FormMain.Config.TypesAttackMelee)
            {
                Debug.Assert(ta.ID != ID);
                Debug.Assert(ta.Name != Name);
            }    
        }

        internal bool IsWeapon { get; }
    }

    internal sealed class TypeAttackRange : TypeAttack
    {
        public TypeAttackRange(XmlNode n) : base(n)
        {
            foreach (TypeAttackRange ta in FormMain.Config.TypesAttackRange)
            {
                Debug.Assert(ta.ID != ID);
                Debug.Assert(ta.Name != Name);
            }
        }
    }

    internal sealed class TypeAttackMagic : TypeAttack
    {
        public TypeAttackMagic(XmlNode n) : base(n)
        {
            foreach (TypeAttackMagic ta in FormMain.Config.TypesAttackMagic)
            {
                Debug.Assert(ta.ID != ID);
                Debug.Assert(ta.Name != Name);
            }
        }
    }
}
