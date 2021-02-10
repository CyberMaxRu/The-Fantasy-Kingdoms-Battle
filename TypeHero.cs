using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Тип героя гильдии
    internal sealed class TypeHero : TypeCreature
    {
        private string nameMeleeWeapon;
        private string nameRangeWeapon;
        private string nameArmour;

        public TypeHero(XmlNode n) : base(n)
        {
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Building = FormMain.Config.FindTypeConstructionWithHero(n.SelectSingleNode("Building").InnerText);
            Building.TrainedHero = this;
            CanBuild = Convert.ToBoolean(n.SelectSingleNode("CanBuild").InnerText);
            DamageToCastle = Convert.ToInt32(n.SelectSingleNode("DamageToCastle").InnerText);
            QuantityArrows = XmlUtils.GetInteger(n.SelectSingleNode("QuantityArrows"));

            //Debug.Assert(Cost > 0);
            Debug.Assert(DamageToCastle >= 0);

            // Проверяем, что таких же ID и наименования нет
            foreach (TypeHero h in FormMain.Config.TypeHeroes)
            {
                if (h.ID == ID)
                {
                    throw new Exception("В конфигурации героев повторяется ID = " + ID);
                }

                if (h.Name == Name)
                {
                    throw new Exception("В конфигурации героев повторяется Name = " + Name);
                }

                if (h.ImageIndex == ImageIndex)
                {
                    throw new Exception("В конфигурации героев повторяется ImageIndex = " + ImageIndex.ToString());
                }

                if (h.DefaultPositionPriority == DefaultPositionPriority)
                {
                    throw new Exception("У героя " + h.Name + " уже указан приоритет " + DefaultPositionPriority.ToString());
                }
            }

            // Загружаем дефолтное оружие и доспехи
            nameMeleeWeapon = XmlUtils.GetString(n.SelectSingleNode("MeleeWeapon"));
            nameRangeWeapon = XmlUtils.GetString(n.SelectSingleNode("RangeWeapon"));
            nameArmour = XmlUtils.GetString(n.SelectSingleNode("Armour"));

            Debug.Assert(nameMeleeWeapon != "");
            Debug.Assert(nameArmour != "");

            if (nameRangeWeapon.Length > 0)
            {
                Debug.Assert(QuantityArrows > 0);
            }
            else
            {
                Debug.Assert(QuantityArrows == 0);
            }

            // Загружаем информацию о переносимых предметах
            XmlNode nc = n.SelectSingleNode("CarryItems");
            if (nc != null)
            {
                Item item;
                int maxQuantity;

                foreach (XmlNode l in nc.SelectNodes("CarryItem"))
                {
                    item = FormMain.Config.FindItem(l.SelectSingleNode("Item").InnerText);
                    maxQuantity = Convert.ToInt32(l.SelectSingleNode("Item").Attributes[0]);

                    Debug.Assert(maxQuantity > 0);

                    // Проверка на уникальность обеспечена Dictionary?
                    //foreach (Item i in CarryItems)
                    //    if (i == item)
                    //        throw new Exception("Предмет " + item.ID + " уже ест в списке переносимых предметов.");

                    CarryItems.Add(item, maxQuantity);
                }
            }
        }

        internal int Cost { get; }
        internal TypeConstructionWithHero Building { get; }
        internal bool CanBuild { get; }
        internal int DamageToCastle { get; }
        internal Weapon WeaponMelee { get; private set; }// Рукопашное оружие
        internal Weapon WeaponRange { get; private set; }// Стрелковое оружие
        internal Armour Armour { get; private set; }// Доспех по умолчанию
        internal Dictionary<Item, int> CarryItems { get; } = new Dictionary<Item, int>();
        internal int QuantityArrows { get; }// Количество стрел

        internal int MaxQuantityItem(Item i)
        {
            return CarryItems.ContainsKey(i) ? CarryItems[i] : 0;
        }

        internal void TuneDeferredLinks()
        {
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
