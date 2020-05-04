using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    internal enum TypeBuilding { Castle, Trade, Other };

    // Класс здания
    internal sealed class Building : Construction
    {
        public Building(XmlNode n) : base (n)
        {
            TypeBuilding = (TypeBuilding)Enum.Parse(typeof(TypeBuilding), n.SelectSingleNode("TypeBuilding").InnerText);
            Position = FormMain.Config.Buildings.Count;

            Debug.Assert(DefaultLevel >= 0);
            Debug.Assert(MaxLevel > 0);
            Debug.Assert(MaxLevel <= 3);
            Debug.Assert(DefaultLevel <= MaxLevel);

            // Проверяем, что таких же ID и наименования нет
            foreach (Building b in FormMain.Config.Buildings)
            {
                if (b.ID == ID)
                    throw new Exception("В конфигурации зданий повторяется ID = " + ID);

                if (b.Name == Name)
                    throw new Exception("В конфигурации зданий повторяется Name = " + Name);

                if (b.ImageIndex == ImageIndex)
                    throw new Exception("В конфигурации зданий повторяется ImageIndex = " + ImageIndex.ToString());
            }
        }

        internal int Position { get; }
        internal TypeBuilding TypeBuilding { get; }
        internal PanelBuilding Panel { get; set; }
    }
}