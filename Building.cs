using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    internal enum TypeBuilding { Castle, Military, Economy, Production, Habitation, Resource, Other };

    // Класс здания
    internal sealed class Building
    {
        public Building(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            ImageIndex = n.SelectSingleNode("ImageIndex") != null ? Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText) : -1;

            /*            switch (PlaceBuilding) 
            {
                case PlaceBuilding.External:
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
                case PlaceBuilding.Internal:
                    Position = FormMain.Config.InternalBuildings.Count;
                    break;
                default:
                    new Exception("Неизвестный тип здания");
                    break;
            }                */
        }

        internal string ID { get; }
        internal string Name { get; }
        internal int ImageIndex { get; }
        internal int Position { get; }
        internal int Cost;
    }
}
