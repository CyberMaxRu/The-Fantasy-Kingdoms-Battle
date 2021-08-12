using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Класс способности
    internal enum TypeAbility { MeleeAttack, RangeAttack, Spell, Buff, Heal, Summon }
    internal enum TypeTarget { Self, EnemyUnit, EnemyBuilding, AllyUnit }// Тип цели для способности
    internal enum Effect { Taunt, Slow }// Эффекты

    internal sealed class Ability : Entity
    {
        private List<string> classesHeroesString = new List<string>();
        public Ability(XmlNode n) : base(n)
        {
            TypeAbility = (TypeAbility)Enum.Parse(typeof(TypeAbility), n.SelectSingleNode("TypeAbility").InnerText);
            TypeTarget = (TypeTarget)Enum.Parse(typeof(TypeTarget), n.SelectSingleNode("TypeTarget").InnerText);
            MinUnitLevel = Convert.ToInt32(n.SelectSingleNode("MinUnitLevel").InnerText);
            Ranged = Convert.ToBoolean(n.SelectSingleNode("Ranged").InnerText);
            MissileVelocity = n.SelectSingleNode("MissileVelocity") != null ? Convert.ToInt32(n.SelectSingleNode("MissileVelocity").InnerText) : 0;
            AoeRadius = XmlUtils.GetInteger(n.SelectSingleNode("AoeRadius"));
            SkillModificator = Convert.ToDouble(n.SelectSingleNode("SkillModif").InnerText);
            CoolDown = Convert.ToInt32(n.SelectSingleNode("CoolDown").InnerText);
            ManaCost = n.SelectSingleNode("ManaCost") != null ? Convert.ToInt32(n.SelectSingleNode("ManaCost").InnerText) : 0;

            // Проверяем, что таких же ID и наименования нет
            foreach (Ability a in FormMain.Config.Abilities)
            {
                if (a.ID == ID)
                    throw new Exception("В конфигурации способностей повторяется ID = " + ID);

                //if (a.Name == Name)
                //    throw new Exception("В конфигурации способностей повторяется Name = " + Name);

                //if (a.ImageIndex == ImageIndex)
                //    throw new Exception("В конфигурации способностей повторяется ImageIndex = " + ImageIndex.ToString());
            }

            // Загружаем эффекты
            XmlNode ne = n.SelectSingleNode("Effects");
            Effect e;

            foreach (XmlNode l in ne.SelectNodes("Effect"))
            {
                e = (Effect)Enum.Parse(typeof(Effect), l.InnerText);

                // Проверяем, что такой эффект не повторяется
                foreach (Effect e2 in Effects)
                {
                    if (e == e2)
                        throw new Exception("Эффект " + e.ToString() + " повторяется в списке эффектов способности.");
                }

                Effects.Add(e);
            }

            switch (TypeAbility)
            {
                case TypeAbility.MeleeAttack:
                    Debug.Assert(TypeTarget != TypeTarget.Self);
                    Debug.Assert(TypeTarget != TypeTarget.AllyUnit);

                    break;
                case TypeAbility.RangeAttack:
                    Debug.Assert(TypeTarget != TypeTarget.Self);
                    Debug.Assert(TypeTarget != TypeTarget.AllyUnit);

                    break;
                case TypeAbility.Buff:
                    Debug.Assert(TypeTarget != TypeTarget.EnemyUnit);
                    Debug.Assert(TypeTarget != TypeTarget.EnemyBuilding);

                    //Debug.Assert(Effects.Count > 0);
                    break;
                case TypeAbility.Spell:
                    break;
                case TypeAbility.Heal:
                    Debug.Assert(TypeTarget != TypeTarget.EnemyUnit);
                    Debug.Assert(TypeTarget != TypeTarget.EnemyBuilding);

                    break;
            }

            // Загружаем классы героев, которые могут использовать способность
            XmlNode nch = n.SelectSingleNode("ClassesHeroes");
            string nameHero;

            foreach (XmlNode l in nch.SelectNodes("ClassHero"))
            {
                nameHero = l.InnerText;

                // Проверяем, что такой класс героев не повторяется
                foreach (string nameHero2 in classesHeroesString)
                {
                    if (nameHero == nameHero2)
                        throw new Exception("Класс героев " + nameHero + " повторяется в списке классов героев способности.");
                }

                classesHeroesString.Add(nameHero);
            }

            Debug.Assert(classesHeroesString.Count > 0);
        }

        internal TypeAbility TypeAbility { get; }
        internal TypeTarget TypeTarget { get; }
        internal int MinUnitLevel { get; }
        internal bool Ranged { get; }
        internal int MissileVelocity { get; }
        internal int AoeRadius { get; }
        internal double SkillModificator { get; }
        internal int ManaCost { get; }
        internal int CoolDown { get; }
        internal List<Effect> Effects { get; } = new List<Effect>();
        internal List<TypeCreature> ClassesHeroes { get; } = new List<TypeCreature>();

        internal void TuneDeferredLinks()
        {
            foreach (string nameHero in classesHeroesString)
                ClassesHeroes.Add(FormMain.Config.FindTypeCreature(nameHero));

            classesHeroesString = null;

            Description += (Description.Length > 0 ? Environment.NewLine : "") + "- Доступно:";

            foreach (TypeCreature u in ClassesHeroes)
            {
                Description += Environment.NewLine + "  - " + u.Name;
            }
        }

        protected override int GetLevel() => 0;
        protected override int GetQuantity() => 0;
        protected override string GetCost()
        {
            switch (TypeAbility)
            {
                case TypeAbility.MeleeAttack:
                    return "удар";
                case TypeAbility.RangeAttack:
                    return "стрел.";
                case TypeAbility.Buff:
                    return "усил";
                case TypeAbility.Spell:
                    return "закл.";
                case TypeAbility.Heal:
                    return "леч.";
                case TypeAbility.Summon:
                    return "призыв";
                default:
                    throw new Exception("Неизвестная способность");
            }
        }

        protected override void DoPrepareHint()
        {
            base.DoPrepareHint();
        }
    }
}
