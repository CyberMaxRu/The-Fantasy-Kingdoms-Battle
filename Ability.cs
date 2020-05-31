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

            // Проверяем, что таких же ID и наименования нет
            foreach (Ability a in FormMain.Config.Abilities)
            {
                if (a.ID == ID)
                    throw new Exception("В конфигурации способностей повторяется ID = " + ID);

                if (a.Name == Name)
                    throw new Exception("В конфигурации способностей повторяется Name = " + Name);

                if (a.ImageIndex == ImageIndex)
                    throw new Exception("В конфигурации способностей повторяется ImageIndex = " + ImageIndex.ToString());
            }
        }

        internal TypeAbility TypeAbility { get; }
        internal TypeAttack TypeAttack { get; }
        internal double SkillModificator { get; }
    }
}
