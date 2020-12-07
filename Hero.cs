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
    internal sealed class Hero
    {
        private string nameMeleeWeapon;
        private string nameRangeWeapon;
        private string nameArmour;

        public Hero(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            if (n.SelectSingleNode("Description") != null)
                Description = n.SelectSingleNode("Description").InnerText.Replace("/", Environment.NewLine);
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Building = FormMain.Config.FindBuilding(n.SelectSingleNode("Building").InnerText);
            Building.TrainedHero = this;
            MaxLevel = Convert.ToInt32(n.SelectSingleNode("MaxLevel").InnerText);
            KindHero = FormMain.Config.FindKindHero(n.SelectSingleNode("KindHero").InnerText);
            CanBuild = Convert.ToBoolean(n.SelectSingleNode("CanBuild").InnerText);
            DamageToCastle = Convert.ToInt32(n.SelectSingleNode("DamageToCastle").InnerText);

            //Debug.Assert(Cost > 0);
            Debug.Assert(ID.Length > 0);
            Debug.Assert(Name.Length > 0);
            Debug.Assert(Description.Length > 0);
            Debug.Assert(ImageIndex >= 0);
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
            }

            // Загружаем дефолтное оружие и доспехи
            nameMeleeWeapon = XmlUtils.GetParamFromXmlString(n.SelectSingleNode("MeleeWeapon"));
            nameRangeWeapon = XmlUtils.GetParamFromXmlString(n.SelectSingleNode("RangeWeapon"));
            nameArmour = XmlUtils.GetParamFromXmlString(n.SelectSingleNode("Armour"));

            if (KindHero.Hired)
            {
                Debug.Assert(nameMeleeWeapon != "");
                Debug.Assert(nameArmour != "");
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
                Ability a;

                foreach (XmlNode l in na.SelectNodes("Ability"))
                {
                    a = FormMain.Config.FindAbility(l.InnerText);

                    // Проверяем, что такая способность не повторяется
                    foreach (Ability a2 in Abilities)
                    {
                        if (a.ID == a2.ID)
                            throw new Exception("Способность " + a.ID + " повторяется в списке способностей героя.");
                    }

                    Abilities.Add(a);
                }
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; }
        internal int ImageIndex { get; }
        internal int Cost { get; }
        internal Building Building { get; }
        internal int MaxLevel { get; }
        internal KindHero KindHero { get; }
        internal bool CanBuild { get; }
        internal int DamageToCastle { get; }
        internal HeroParameters ParametersByHire { get; }// Параметры при найме героя
        internal ConfigNextLevelHero ConfigNextLevel { get; }
        internal List<Ability> Abilities { get; } = new List<Ability>();// Способности героя
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
