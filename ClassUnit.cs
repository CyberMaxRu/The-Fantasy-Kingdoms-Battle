using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Класс класса юнита
    internal sealed class ClassUnit
    {
        public ClassUnit(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;

            // Проверяем, что таких же ID и наименования нет
            foreach (ClassUnit cu in FormMain.Config.ClassesUnits)
            {
                if (cu.ID == ID)
                {
                    throw new Exception("В конфигурации классов юнитов повторяется ID = " + ID);
                }

                if (cu.Name == Name)
                {
                    throw new Exception("В конфигурации классов юнитов повторяется Name = " + Name);
                }
            }
        }

        internal string ID { get; }
        internal string Name { get; }
    }
}
