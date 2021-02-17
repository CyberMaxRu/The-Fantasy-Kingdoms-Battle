using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Тип героя гильдии
    internal sealed class TypeHero : TypeCreature
    {
        public TypeHero(XmlNode n) : base(n)
        {
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Building = FormMain.Config.FindTypeConstructionOfKingdom(n.SelectSingleNode("Building").InnerText);
            Building.TrainedHero = this;
            CanBuild = Convert.ToBoolean(n.SelectSingleNode("CanBuild").InnerText);

            //Debug.Assert(Cost > 0);

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
        internal TypeConstruction Building { get; }
        internal bool CanBuild { get; }
        internal Dictionary<Item, int> CarryItems { get; } = new Dictionary<Item, int>();

        internal int MaxQuantityItem(Item i)
        {
            return CarryItems.ContainsKey(i) ? CarryItems[i] : 0;
        }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();
            /*foreach (Ability a in Abilities)
                if (a.ClassesHeroes.IndexOf(this) == -1)
                    throw new Exception("Класс героя " + ID + " отсутствует в списке доступных для способности " + a.ID);
            */
        }
    }
}
