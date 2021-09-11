using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum NameTypeAbility { MeleeAttack, RangeAttack, Spell, Buff, Debuff, Heal, Summon, Passive, Aura }

    // Класс типа способности
    internal sealed class DescriptorTypeAbility : Descriptor
    {
        public DescriptorTypeAbility(XmlNode n) : base()
        {
            ID = XmlUtils.GetStringNotNull(n, "ID");
            Name = XmlUtils.GetStringNotNull(n, "Name");
            ShortName = XmlUtils.GetStringNotNull(n, "ShortName");

            NameTypeAbility = (NameTypeAbility)Enum.Parse(typeof(NameTypeAbility), ID);
            Pos = Config.TypeAbilities.Count + 1;
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string ShortName { get; }
        internal NameTypeAbility NameTypeAbility { get; }
        internal int Pos { get; }
    }
}
