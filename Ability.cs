using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Класс способности
    internal enum TypeAbility { Attack, Spell }

    internal sealed class Ability : Entity
    {
        public Ability(XmlNode n) : base(n)
        {
            TypeAbility = (TypeAbility)Enum.Parse(typeof(TypeAbility), n.SelectSingleNode("TypeAbility").InnerText);
            TypeAttack = (TypeAttack)Enum.Parse(typeof(TypeAttack), n.SelectSingleNode("TypeAttack").InnerText);
            SkillModificator = Convert.ToDouble(n.SelectSingleNode("SkillModif").InnerText);
        }

        internal TypeAbility TypeAbility { get; }
        internal TypeAttack TypeAttack { get; }
        internal double SkillModificator { get; }
    }
}
