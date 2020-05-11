using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{

    // Класс предмета
    internal sealed class Item
    {
        public Item(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            Description = n.SelectSingleNode("Description").InnerText.Replace("/", Environment.NewLine);
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            TypeItem = FormMain.Config.FindTypeItem(n.SelectSingleNode("TypeItem").InnerText);
            Building = FormMain.Config.FindBuilding(n.SelectSingleNode("Building").InnerText);
            CostExamine = Convert.ToInt32(n.SelectSingleNode("CostExamine").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);            
            TypeAttack = n.SelectSingleNode("TypeAttack") == null ? TypeAttack.None : (TypeAttack)Enum.Parse(typeof(TypeAttack), n.SelectSingleNode("TypeAttack").InnerText);

            Position = FormMain.Config.Items.Count;

            // Проверяем, что таких же ID и наименования нет
            foreach (Item i in FormMain.Config.Items)
            {
                if (i.ID == ID)
                    throw new Exception("В конфигурации предметов повторяется ID = " + ID);

                if (i.Name == Name)
                    throw new Exception("В конфигурации предметов повторяется Name = " + Name);

                if (i.ImageIndex == ImageIndex)
                    throw new Exception("В конфигурации предметов повторяется ImageIndex = " + ImageIndex.ToString());
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; }
        internal int ImageIndex { get; }
        internal TypeItem TypeItem { get; }
        internal TypeAttack TypeAttack { get; }
        internal int Position { get; }
        internal Building Building { get; }
        internal int CostExamine { get; }
        internal int Cost { get; }
    }
}