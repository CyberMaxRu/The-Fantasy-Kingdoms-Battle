using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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

        internal int ID { get; }
        internal string Name { get; }
        internal Point Center { get; set; }
    }
}