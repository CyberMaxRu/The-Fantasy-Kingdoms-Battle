using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    internal enum CategoryItem { Weapon, Armour, Thing, Potion }

    // Класс типа предмета
    internal sealed class TypeItem
    {
        public TypeItem(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Category = (CategoryItem)Enum.Parse(typeof(CategoryItem), n.SelectSingleNode("Category").InnerText);
            Required = Convert.ToBoolean(n.SelectSingleNode("Required").InnerText);
            Single = Convert.ToBoolean(n.SelectSingleNode("Single").InnerText);

            // Проверяем, что таких же ID и наименования нет
            foreach (TypeItem ti in FormMain.Config.TypeItems)
            {
                if (ti.ID == ID)
                {
                    throw new Exception("В конфигурации типов предметов повторяется ID = " + ID);
                }
            }
        }

        internal string ID { get; }
        internal CategoryItem Category { get; }
        internal bool Required { get; }
        internal bool Single { get; }// Тип предмета только в одной ячейке
    }
}
