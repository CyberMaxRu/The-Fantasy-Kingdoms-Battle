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
    internal sealed class TypeCreature
    {
        public TypeCreature(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;

            // Проверяем, что таких же ID и наименования нет
            foreach (TypeCreature tu in FormMain.Config.TypeUnits)
            {
                if (tu.ID == ID)
                    throw new Exception("В конфигурации типов существ повторяется ID = " + ID);

                if (tu.Name == Name)
                    throw new Exception("В конфигурации типов существ повторяется Name = " + Name);
            }
        }

        internal string ID { get; }
        internal string Name { get; }       
    }
}
