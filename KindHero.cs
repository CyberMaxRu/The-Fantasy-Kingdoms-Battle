using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Класс вида героя
    internal sealed class KindHero
    {
        public KindHero(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Hired = Convert.ToBoolean(n.SelectSingleNode("Hired").InnerText);

            // Проверяем, что таких же ID нет
            foreach (KindHero kh in FormMain.Config.KindHeroes)
            {
                if (kh.ID == ID)
                {
                    throw new Exception("В конфигурации видов героев повторяется ID = " + ID);
                }
            }
        }

        internal string ID { get; }
        internal bool Hired { get; }
    }
}
