using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс исследования
    internal sealed class Research
    {
        public Research(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Item = FormMain.Config.FindItem(n.SelectSingleNode("Item").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);

            // Проверяем, что таких же ID и предмета нет
            foreach (Research r in FormMain.Config.Researches)
            {
                if (r.ID == ID)
                    throw new Exception("В конфигурации исследований повторяется ID = " + ID);

                if (Item == r.Item)
                    throw new Exception("В конфигурации исследований повторяется Item = " + Item);
            }
        }

        internal string ID { get; }// Код исследования
        internal Item Item { get; }// Получаемый предмет
        internal int Cost { get; }// Стоимость исследования
        internal List<Requirement> Requirements { get; } = new List<Requirement>();
    }
}
