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

namespace Fantasy_Kingdoms_Battle
{
    // Класс исследования
    internal sealed class Research
    {
        private string nameItem;
        private string nameAbility;
        private string nameGroupWeapon;
        private string nameGroupArmour;
        private string nameTypeConstruction;

        public Research(XmlNode n)
        {
            Coord = new Point(Convert.ToInt32(n.SelectSingleNode("PosX").InnerText) - 1, Convert.ToInt32(n.SelectSingleNode("PosY").InnerText) - 1);
            Layer = Convert.ToInt32(n.SelectSingleNode("Layer").InnerText) - 1;
            nameItem = XmlUtils.GetString(n.SelectSingleNode("Item"));
            nameAbility = XmlUtils.GetString(n.SelectSingleNode("Ability"));
            nameGroupWeapon = XmlUtils.GetString(n.SelectSingleNode("GroupWeapon"));
            nameGroupArmour = XmlUtils.GetString(n.SelectSingleNode("GroupArmour"));
            nameTypeConstruction = XmlUtils.GetString(n.SelectSingleNode("TypeConstruction"));
            Cost = XmlUtils.GetInteger(n.SelectSingleNode("Cost"));

            // Загружаем требования
            XmlUtils.LoadRequirements(Requirements, n);

            Debug.Assert((nameItem != "") || (nameAbility != "") || (nameGroupWeapon != "") || (nameGroupArmour != "") || (nameTypeConstruction != ""));
            Debug.Assert(!((nameItem != "") && (nameAbility != "") && (nameGroupWeapon != "") && (nameGroupArmour != "") && (nameTypeConstruction != "")));
        }

        internal Point Coord { get; }// Координаты исследования
        internal int Layer { get; }// Визуальный слой исследования
        internal Entity Entity { get; private set; }// Получаемая сущность
        internal TypeConstruction TypeConstruction { get; private set; }// Строимое сооружение
        internal int Cost { get; }// Стоимость исследования
        internal List<Requirement> Requirements { get; } = new List<Requirement>();

        internal void FindItem()
        {
            if (nameItem != "")
            {
                Entity = FormMain.Config.FindItem(nameItem);

                foreach (Requirement r in Requirements)
                    r.FindConstruction();
            }
            else if (nameAbility != "")
            {
                Entity = FormMain.Config.FindAbility(nameAbility);

                foreach (Requirement r in Requirements)
                    r.FindConstruction();
            }
            else if (nameGroupWeapon != "")
            {
                Entity = FormMain.Config.FindGroupWeapon(nameGroupWeapon);

                foreach (Requirement r in Requirements)
                    r.FindConstruction();
            }
            else if (nameGroupArmour != "")
            {
                Entity = FormMain.Config.FindGroupArmour(nameGroupArmour);

                foreach (Requirement r in Requirements)
                    r.FindConstruction();
            }
            else if (nameTypeConstruction != "")
            {
                TypeConstruction = FormMain.Config.FindTypeConstructionOfKingdom(nameTypeConstruction);
            }

            nameItem = null;
            nameAbility = null;
            nameGroupWeapon = null;
            nameGroupArmour = null;
            nameTypeConstruction = null;
        }
    }
}
