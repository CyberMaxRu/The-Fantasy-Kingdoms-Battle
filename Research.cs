using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Deployment.Internal;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Класс исследования
    internal sealed class Research
    {
        private string nameItem;
        private string nameAbility;

        public Research(XmlNode n)
        {
            Coord = new Point(Convert.ToInt32(n.SelectSingleNode("PosX").InnerText) - 1, Convert.ToInt32(n.SelectSingleNode("PosY").InnerText) - 1);
            Layer = Convert.ToInt32(n.SelectSingleNode("Layer").InnerText) - 1;
            nameItem = n.SelectSingleNode("Item") != null ? n.SelectSingleNode("Item").InnerText : "";
            nameAbility = n.SelectSingleNode("Ability") != null ? n.SelectSingleNode("Ability").InnerText : "";
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);

            // Загружаем требования
            Utils.LoadRequirements(Requirements, n);

            Debug.Assert((nameItem != "") || (nameAbility != ""));
            Debug.Assert(!((nameItem != "") && (nameAbility != "")));
        }

        internal Point Coord { get; }// Координаты исследования
        internal int Layer { get; }// Визуальный слой исследования
        internal Entity Entity { get; private set; }// Получаемая сущность
        internal int Cost { get; }// Стоимость исследования
        internal List<Requirement> Requirements { get; } = new List<Requirement>();

        internal void FindItem()
        {
            if (nameItem != "")
            {
                Entity = FormMain.Config.FindItem(nameItem);
                nameItem = null;

                foreach (Requirement r in Requirements)
                    r.FindBuilding();
            }
            else if (nameAbility != "")
            {
                Entity = FormMain.Config.FindAbility(nameAbility);
                nameAbility = null;

                foreach (Requirement r in Requirements)
                    r.FindBuilding();
            }
        }
    }
}
