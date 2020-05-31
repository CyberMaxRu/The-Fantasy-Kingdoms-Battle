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
    internal enum TypeAttack { Melee, Missile, None }

    // Конфигурация типов предметов, которые героя может носить
    internal sealed class CarryTypeItem
    {
        public CarryTypeItem(XmlNode n)
        {
            TypeItem = FormMain.Config.FindTypeItem(n.SelectSingleNode("TypeItem").InnerText);
            MaxQuantity = Convert.ToInt32(n.SelectSingleNode("MaxQuantity").InnerText);

            Debug.Assert(MaxQuantity > 0);
        }

        internal TypeItem TypeItem { get; }
        internal int MaxQuantity { get; }
    }

    internal sealed class Slot
    {
        public Slot(Hero h, XmlNode n)
        {
            Pos = Convert.ToInt32(n.SelectSingleNode("Pos").InnerText) - 1;
            TypeItem = FormMain.Config.FindTypeItem(n.SelectSingleNode("TypeItem").InnerText);
            if (n.SelectSingleNode("DefaultItem") != null)
                DefaultItem = FormMain.Config.FindItem(n.SelectSingleNode("DefaultItem").InnerText);

            if ((TypeItem.Required == true) && (DefaultItem == null))
                throw new Exception("У слота " + Pos.ToString() + " " + h.ID + " не указано дефолтное значение.");

            if ((TypeItem.Required == false) && (DefaultItem != null))
                throw new Exception("У слота " + Pos.ToString() + " " + h.ID + " указано дефолтное значение.");

            if ((DefaultItem != null) && (DefaultItem.TypeItem != TypeItem))
                throw new Exception("У слота " + Pos.ToString() + " " + h.ID + " у дефолтного предмета несовместимый тип.");
        }

        internal int Pos { get; }
        internal TypeItem TypeItem { get; }
        internal Item DefaultItem { get; }
    }

    // Класс героя гильдии    
    internal sealed class Hero
    {
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

            switch (KindHero.TypeAttack)
            {
                case TypeAttack.Melee:
                    //Debug.Assert(DamageToCastle > 0);
                    break;
                case TypeAttack.Missile:
                    Debug.Assert(DamageToCastle > 0);
                    break;
                case TypeAttack.None:
                    Debug.Assert(DamageToCastle == 0); 
                    break;
                default:
                    throw new Exception("Неизвестный тип атаки.");
            }

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

            // Загружаем информацию о слотах
            Slots = new Slot[FormMain.SLOT_IN_INVENTORY];

            XmlNode nl = n.SelectSingleNode("Slots");
            if (nl != null)
            {
                Slot slot;

                foreach (XmlNode l in nl.SelectNodes("Slot"))
                {
                    slot = new Slot(this, l);
                    Debug.Assert(Slots[slot.Pos] == null);

                    Slots[slot.Pos] = slot;
                }

                for (int i = 0; i < FormMain.SLOT_IN_INVENTORY; i++)
                {
                    if (Slots[i] == null)
                        throw new Exception("Не указан слот " + i.ToString());
                }
            }

            // Загружаем информацию о переносимых предметах
            XmlNode nc = n.SelectSingleNode("CarryTypeItems");
            if (nc != null)
            {
                CarryTypeItem cti;

                foreach (XmlNode l in nl.SelectNodes("CarryTypeItem"))
                {
                    cti = new CarryTypeItem(l);

                    // Проверяем, что такой тип предмета не повторяется
                    foreach (CarryTypeItem cti2 in CarryTypeItems)
                    {
                        if (cti.TypeItem == cti2.TypeItem)
                            throw new Exception("Тип предмета " + cti.TypeItem.ID + " повторяется в списке переносимых.");
                    }

                    CarryTypeItems.Add(cti);
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
        internal Slot[] Slots { get; }
        internal List<Ability> Abilities { get; } = new List<Ability>();// Способности героя
        internal List<CarryTypeItem> CarryTypeItems { get; } = new List<CarryTypeItem>();

        internal int MaxQuantityTypeItem(TypeItem ti)
        {
            foreach (CarryTypeItem cti in CarryTypeItems)
            {
                if (cti.TypeItem == ti)
                {
                    return cti.MaxQuantity;
                }
            }

            return 0;
        }
    }
}
