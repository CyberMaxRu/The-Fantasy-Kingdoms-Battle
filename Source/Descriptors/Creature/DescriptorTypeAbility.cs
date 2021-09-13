using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum NameTypeAbility { MeleeAttack, RangeAttack, MagicAttack, Spell, Buff, Debuff, Heal, Summon, Permanent, Aura }

    // Класс типа способности
    internal sealed class DescriptorTypeAbility : DescriptorWithID
    {
        public DescriptorTypeAbility(XmlNode n) : base(n)
        {
            ShortName = XmlUtils.GetStringNotNull(n, "ShortName");
            NameTypeAbility = (NameTypeAbility)Enum.Parse(typeof(NameTypeAbility), ID);
            Pos = Config.TypeAbilities.Count + 1;
        }

        internal string ShortName { get; }
        internal NameTypeAbility NameTypeAbility { get; }
        internal int Pos { get; }
    }
}
