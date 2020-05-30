using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс исследования
    internal sealed class Research
    {
        private string nameItem;

        public Research(XmlNode n)
        {
            Coord = new Point(Convert.ToInt32(n.SelectSingleNode("PosX").InnerText) - 1, Convert.ToInt32(n.SelectSingleNode("PosY").InnerText) - 1);
            Layer = Convert.ToInt32(n.SelectSingleNode("Layer").InnerText) - 1;
            nameItem = n.SelectSingleNode("Item").InnerText;
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
        }

        internal Point Coord { get; }// Координаты исследования
        internal int Layer { get; }// Визуальный слой исследования
        internal Item Item { get; private set; }// Получаемый предмет
        internal int Cost { get; }// Стоимость исследования
        internal List<Requirement> Requirements { get; } = new List<Requirement>();

        internal void FindItem()
        {
            Item = FormMain.Config.FindItem(nameItem);
            nameItem = null;
        }
    }
}
