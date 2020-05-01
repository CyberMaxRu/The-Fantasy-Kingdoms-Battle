using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    internal enum TypeBuilding { Castle, Economy};

    // Класс здания
    internal sealed class Building
    {
        public Building(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            TypeBuilding = (TypeBuilding)Enum.Parse(typeof(TypeBuilding), n.SelectSingleNode("TypeBuilding").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Position = FormMain.Config.Buildings.Count;

            // Проверяем, что таких же ID и наименования нет
            foreach (Building b in FormMain.Config.Buildings)
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
        }

        internal string ID { get; }
        internal string Name { get; }
        internal int ImageIndex { get; }
        internal int Position { get; }
        internal TypeBuilding TypeBuilding { get; }
        internal int Cost { get; }

        internal PanelBuilding Panel { get; set; }
    }
}
