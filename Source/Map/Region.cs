using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;    

namespace Fantasy_Kingdoms_Battle
{
    // Класс области карты
    internal sealed class Region
    {
        private static int id = 1;
            
        public Region()
        {
            ID = id++;
            Name = "Регион #" + ID.ToString();
        }

        public Region(XmlNode n)
        {
            ID = XmlUtils.GetIntegerNotNull(n, "ID");
            Name = XmlUtils.GetStringNotNull(n, "Name");
        }

        internal int ID { get; }
        internal string Name { get; }
        internal Point Center { get; set; }
        internal List<Point> Points { get; } = new List<Point>();
    }
}