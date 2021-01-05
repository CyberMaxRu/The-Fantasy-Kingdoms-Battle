using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Тип юнита
    class TypeUnit
    {
        public TypeUnit(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;

            // Проверяем, что таких же ID и наименования нет
            foreach (TypeUnit tu in FormMain.Config.TypeUnits)
            {
                if (tu.ID == ID)
                    throw new Exception("В конфигурации типов юнитов повторяется ID = " + ID);

                if (tu.Name == Name)
                    throw new Exception("В конфигурации типов повторяется Name = " + Name);
            }
        }

        internal string ID { get; }
        internal string Name { get; }       
    }
}
