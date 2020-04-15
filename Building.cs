using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    internal enum TypeBuilding { Internal, External };

    // Класс здания
    internal sealed class Building
    {
        public Building(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            TypeBuilding = (TypeBuilding)Enum.Parse(typeof(TypeBuilding), n.SelectSingleNode("TypeBuilding").InnerText);
            //ImageIndex = n.SelectSingleNode("ImageIndex") != null ? Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText) : -1;
            
            switch (TypeBuilding) 
            {
                case TypeBuilding.External:
                    Position = FormMain.Config.ExternalBuildings.Count;

                    // Проверяем, что таких же ID и наименования нет
                    foreach (Building b in FormMain.Config.ExternalBuildings)
                    {
                        if (b.ID == ID)
                        {
                            throw new Exception("В конфигурации зданий повторяется ID = " + ID);
                        }

                        if (b.Name == Name)
                        {
                            throw new Exception("В конфигурации зданий повторяется Name = " + Name);
                        }
                    }
                    break;
                case TypeBuilding.Internal:
                    Position = FormMain.Config.InternalBuildings.Count;
                    break;
                default:
                    new Exception("Неизвестный тип здания");
                    break;
            }                

            // Добавляем стоимость постройки
            XmlNode l;
            Resource r;
            int value;
            l = n.SelectSingleNode("Cost");
            for (int k = 0; k < l.ChildNodes.Count; k++)
            {
                r = FormMain.Config.FindResource(l.ChildNodes[k].LocalName);
                value = Convert.ToInt32(l.ChildNodes[k].InnerText);
                if (value <= 0)
                    throw new Exception("У здания " + ID + " стоимость " + r.ToString() + " меньше или равна нолю (" + value.ToString() + ").");

                Cost[r] = value;
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal int Position { get; }
        internal TypeBuilding TypeBuilding { get; }
        internal Dictionary<Resource, int> Cost = new Dictionary<Resource, int>();
    }
}
