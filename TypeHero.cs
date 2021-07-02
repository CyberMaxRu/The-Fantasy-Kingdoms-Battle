using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Тип героя гильдии
    internal sealed class TypeHero : TypeCreature
    {
        private string nameFromTypeHero;
        private string surnameFromTypeHero;

        public TypeHero(XmlNode n) : base(n)
        {
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Construction = FormMain.Config.FindTypeConstructionOfKingdom(n.SelectSingleNode("Construction").InnerText);
            Construction.TrainedHero = this;
            CanBuild = Convert.ToBoolean(n.SelectSingleNode("CanBuild").InnerText);
            PrefixName = XmlUtils.GetString(n.SelectSingleNode("PrefixName"));
            nameFromTypeHero = XmlUtils.GetString(n.SelectSingleNode("NameFromTypeHero"));
            surnameFromTypeHero = XmlUtils.GetString(n.SelectSingleNode("SurnameFromTypeHero"));

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

            // Загружаем имена и фамилии
            LoadName("Names", "Name", Names);
            LoadName("Surnames", "Surname", Surnames);

            if (Cost > 0)
            {
                Debug.Assert(((Names.Count > 0) && (nameFromTypeHero.Length == 0)) || ((Names.Count == 0) && (nameFromTypeHero.Length > 0)));
                Debug.Assert(((Surnames.Count > 0) && (surnameFromTypeHero.Length == 0)) || ((Surnames.Count == 0) && (surnameFromTypeHero.Length >= 0)));
            }

            void LoadName(string nodes, string node, List<string> list)
            {
                nc = n.SelectSingleNode(nodes);
                string name;
                if (nc != null)
                {
                    foreach (XmlNode l in nc.SelectNodes(node))
                    {
                        name = l.InnerText;

                        Debug.Assert(name != null);
                        Debug.Assert(name.Length > 1);
                        Debug.Assert(list.IndexOf(name) == -1);

                        list.Add(name);
                    }
                }
            }
        }

        internal int Cost { get; }
        internal TypeConstruction Construction { get; }
        internal bool CanBuild { get; }
        internal Dictionary<Item, int> CarryItems { get; } = new Dictionary<Item, int>();
        internal string PrefixName { get; }
        internal List<string> Names { get; } = new List<string>();
        internal List<string> Surnames { get; } = new List<string>();
        internal TypeHero NameFromTypeHero { get; private set; }
        internal TypeHero SurnameFromTypeHero { get; private set; }

        internal int MaxQuantityItem(Item i)
        {
            return CarryItems.ContainsKey(i) ? CarryItems[i] : 0;
        }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            if (nameFromTypeHero != "")
                NameFromTypeHero = FormMain.Config.FindTypeHero(nameFromTypeHero);
            if (surnameFromTypeHero != "")
                SurnameFromTypeHero = FormMain.Config.FindTypeHero(surnameFromTypeHero);

            nameFromTypeHero = null;
            surnameFromTypeHero = null;

            /*foreach (Ability a in Abilities)
                if (a.ClassesHeroes.IndexOf(this) == -1)
                    throw new Exception("Класс героя " + ID + " отсутствует в списке доступных для способности " + a.ID);
            */
        }
    }
}
