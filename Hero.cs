using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс героя гильдии    
    internal sealed class Hero : KindCreature
    {
        private string nameMeleeWeapon;
        private string nameRangeWeapon;
        private string nameArmour;

        public Hero(XmlNode n) : base(n)
        {
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Building = FormMain.Config.FindBuilding(n.SelectSingleNode("Building").InnerText);
            Building.TrainedHero = this;
            KindHero = FormMain.Config.FindKindHero(n.SelectSingleNode("KindHero").InnerText);
            CanBuild = Convert.ToBoolean(n.SelectSingleNode("CanBuild").InnerText);
            DamageToCastle = Convert.ToInt32(n.SelectSingleNode("DamageToCastle").InnerText);

            //Debug.Assert(Cost > 0);
            Debug.Assert(DamageToCastle >= 0);

            // Проверяем, что таких же ID и наименования нет
            foreach (Hero h in FormMain.Config.Heroes)
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

                if (KindHero.Hired && (h.DefaultPositionPriority == DefaultPositionPriority))
                {
                    throw new Exception("У героя " + h.Name + " уже указан приоритет " + DefaultPositionPriority.ToString());
                }
            }

            // Загружаем дефолтное оружие и доспехи
            nameMeleeWeapon = XmlUtils.GetString(n.SelectSingleNode("MeleeWeapon"));
            nameRangeWeapon = XmlUtils.GetString(n.SelectSingleNode("RangeWeapon"));
            nameArmour = XmlUtils.GetString(n.SelectSingleNode("Armour"));

            if (KindHero.Hired)
            {
                Debug.Assert(nameMeleeWeapon != "");
                Debug.Assert(nameArmour != "");
            }
            else
            {
                Debug.Assert(DefaultPositionPriority == 0);
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
            else
            {
                Debug.Assert(KindHero.Hired == false);
            }

        }

        internal int Cost { get; }
        internal Building Building { get; }
        internal KindHero KindHero { get; }
        internal bool CanBuild { get; }
        internal int DamageToCastle { get; }
        internal Weapon WeaponMelee { get; private set; }// Рукопашное оружие
        internal Weapon WeaponRange { get; private set; }// Стрелковое оружие
        internal Armour Armour { get; private set; }// Доспех по умолчанию
        internal Dictionary<Item, int> CarryItems { get; } = new Dictionary<Item, int>();

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
