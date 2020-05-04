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
    internal sealed class Slot
    {
        public Slot(Hero h, XmlNode n)
        {
            Pos = Convert.ToInt32(n.SelectSingleNode("Pos").InnerText) - 1;
            TypeItem = FormMain.Config.FindTypeItem(n.SelectSingleNode("TypeItem").InnerText);
            MaxQuantity = Convert.ToInt32(n.SelectSingleNode("MaxQuantity").InnerText);
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
        internal int MaxQuantity { get; }
        internal Item DefaultItem { get; }
    }

    // Класс героя гильдии    
    internal sealed class Hero
    {
        public Hero(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Building = FormMain.Config.FindBuilding(n.SelectSingleNode("Building").InnerText);
            Building.TrainedHero = this;
            MaxLevel = Convert.ToInt32(n.SelectSingleNode("MaxLevel").InnerText);

            Debug.Assert(Cost > 0);
            Debug.Assert(MaxLevel > 0);

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
            Debug.Assert(nl != null);

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

        internal string ID { get; }
        internal string Name { get; }
        internal int ImageIndex { get; }
        internal int Cost { get; }
        internal Building Building { get; }
        internal int MaxLevel { get; }
        
        internal Slot[] Slots { get; }
    }
}
