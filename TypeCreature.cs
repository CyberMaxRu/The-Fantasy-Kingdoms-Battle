using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый тип существа
    internal abstract class TypeCreature : TypeObject
    {
        private string nameMeleeWeapon;
        private string nameRangeWeapon;
        private string nameArmour;

        public TypeCreature(XmlNode n) : base(n)
        {
            KindCreature = FormMain.Config.FindKindCreature(XmlUtils.GetStringNotNull(n.SelectSingleNode("KindCreature")));
            MaxLevel = XmlUtils.GetInteger(n.SelectSingleNode("MaxLevel"));
            DefaultPositionPriority = XmlUtils.GetInteger(n.SelectSingleNode("DefaultPositionPriority"));
            QuantityArrows = XmlUtils.GetInteger(n.SelectSingleNode("QuantityArrows"));
            TypeAttackMelee = FormMain.Config.FindTypeAttackMelee(XmlUtils.GetString(n.SelectSingleNode("TypeAttackMelee")));
            string typeAttackRange = XmlUtils.GetString(n.SelectSingleNode("TypeAttackRange"));
            if (typeAttackRange.Length > 0)
                TypeAttackRange = FormMain.Config.FindTypeAttackRange(typeAttackRange); 
            if (n.SelectSingleNode("PersistentState") != null)
                PersistentStateHeroAtMap = FormMain.Config.FindStateCreature(XmlUtils.GetStringNotNull(n.SelectSingleNode("PersistentState")));
            else
                PersistentStateHeroAtMap = FormMain.Config.FindStateCreature(NameStateCreature.Nothing.ToString());

            Debug.Assert(MaxLevel >= 1);
            Debug.Assert(MaxLevel <= 100);
            Debug.Assert(DefaultPositionPriority >= 0);
            Debug.Assert(DefaultPositionPriority <= 1000);

            // Загружаем основные параметры
            if (n.SelectSingleNode("BaseParameters") != null)
            {
                ParametersByHire = new HeroParameters(n.SelectSingleNode("BaseParameters"));

                //
                if (n.SelectSingleNode("NextLevel") != null)
                    ConfigNextLevel = new ConfigNextLevelHero(n.SelectSingleNode("NextLevel"));
            }

            // Загружаем дефолтные способности
            XmlNode na = n.SelectSingleNode("Abilities");
            if (na != null)
            {
                TypeAbility a;

                foreach (XmlNode l in na.SelectNodes("Ability"))
                {
                    a = FormMain.Config.FindAbility(l.InnerText);

                    // Проверяем, что такая способность не повторяется
                    foreach (TypeAbility a2 in Abilities)
                    {
                        if (a.ID == a2.ID)
                            throw new Exception("Способность " + a.ID + " повторяется в списке способностей героя.");
                    }

                    Abilities.Add(a);
                }
            }

            // Загружаем дефолтное оружие и доспехи
            nameMeleeWeapon = XmlUtils.GetString(n.SelectSingleNode("MeleeWeapon"));
            nameRangeWeapon = XmlUtils.GetString(n.SelectSingleNode("RangeWeapon"));
            nameArmour = XmlUtils.GetString(n.SelectSingleNode("Armour"));

            //Debug.Assert(nameMeleeWeapon != "");
            //Debug.Assert(nameArmour != "");

            if (nameRangeWeapon.Length > 0)
            {
                Debug.Assert(QuantityArrows > 0);
            }
            else
            {
                Debug.Assert(QuantityArrows == 0);
            }
        }

        internal KindCreature KindCreature { get; }// Вид существа
        internal int MaxLevel { get; }// Максимальный уровень существа
        internal StateCreature PersistentStateHeroAtMap { get; set; }
        internal HeroParameters ParametersByHire { get; }// Параметры при создании существа
        internal ConfigNextLevelHero ConfigNextLevel { get; }
        internal List<TypeAbility> Abilities { get; } = new List<TypeAbility>();// Способности существа
        internal int DefaultPositionPriority { get; private set; }// Приоритет расположения на поле боя по умолчанию
        internal TypeAttackMelee TypeAttackMelee { get;}// Тип рукопашной атаки
        internal TypeAttackRange TypeAttackRange { get; }// Тип дистанционной атаки
        internal Weapon WeaponMelee { get; private set; }// Рукопашное оружие
        internal Weapon WeaponRange { get; private set; }// Стрелковое оружие
        internal Armour Armour { get; private set; }// Доспех по умолчанию
        internal int QuantityArrows { get; }// Количество стрел

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            foreach (TypeAbility a in Abilities)
            {
                Debug.Assert(a.ClassesHeroes.IndexOf(this) != -1, $"Типу существа {ID} не доступна стартовая способность {a.ID}.");
            }

            // Загружаем дефолтное оружие и доспехи
            if (nameMeleeWeapon.Length > 0)
            {
                WeaponMelee = FormMain.Config.FindWeapon(nameMeleeWeapon);
                nameMeleeWeapon = null;

                Debug.Assert(WeaponMelee.ClassHero == this);
            }

            if (nameRangeWeapon.Length > 0)
            {
                WeaponRange = FormMain.Config.FindWeapon(nameRangeWeapon);
                nameRangeWeapon = null;

                Debug.Assert(WeaponMelee.ClassHero == this);
            }

            if (nameArmour.Length > 0)
            {
                Armour = FormMain.Config.FindArmour(nameArmour);
                nameArmour = null;

                Debug.Assert(Armour.ClassHero == this);
            }

            /*foreach (Ability a in Abilities)
                if (a.ClassesHeroes.IndexOf(this) == -1)
                    throw new Exception("Класс героя " + ID + " отсутствует в списке доступных для способности " + a.ID);
            */
        }
    }
}