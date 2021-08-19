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

        }
    }

    internal sealed class TypeAttackRange : TypeAttack
    {
        public TypeAttackRange(XmlNode n) : base(n)
        {

        }
    }
}
