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
    internal sealed class TypeCellMenu
    {
        private string nameTypeObject;
        public TypeCellMenu(XmlNode n)
        {
            Coord = new Point(XmlUtils.GetIntegerNotNull(n.SelectSingleNode("PosX")) - 1, XmlUtils.GetIntegerNotNull(n.SelectSingleNode("PosY")) - 1);
            Layer = XmlUtils.GetIntegerNotNull(n.SelectSingleNode("Layer")) - 1;
            nameTypeObject = XmlUtils.GetString(n.SelectSingleNode("TypeObject"));
            Cost = XmlUtils.GetInteger(n.SelectSingleNode("Cost"));
            XmlUtils.LoadRequirements(Requirements, n);

            Debug.Assert(Coord.X >= 0);
            Debug.Assert(Coord.X <= FormMain.Config.PlateWidth - 1);
            Debug.Assert(Coord.Y >= 0);
            Debug.Assert(Coord.Y <= FormMain.Config.PlateHeight - 1);
            Debug.Assert(Layer >= 0);
            Debug.Assert(Layer <= 4);
            Debug.Assert(nameTypeObject.Length > 0);
        }
    
        internal Point Coord { get; }// Координаты ячейки
        internal int Layer { get; }// Визуальный слой ячейки
        internal Entity Entity { get; set; }// Получаемая сущность
        internal int Cost { get; set; }// Стоимость
        internal TypeConstruction TypeConstruction { get; set; }// Строимое сооружение
        internal List<Requirement> Requirements { get; } = new List<Requirement>();

        internal void TuneDeferredLinks()
        {
            Entity = FormMain.Config.FindItem(nameTypeObject, false);

            if (Entity is null)
                Entity = FormMain.Config.FindAbility(nameTypeObject, false);

            if (Entity is null)
                Entity = FormMain.Config.FindGroupWeapon(nameTypeObject, false);

            if (Entity is null)
                Entity = FormMain.Config.FindGroupArmour(nameTypeObject, false);

            if (Entity is null)
                TypeConstruction = FormMain.Config.FindTypeConstructionOfKingdom(nameTypeObject, false);

            if ((Entity is null) && (TypeConstruction is null))
                throw new Exception("Сущность " + nameTypeObject + " не найдена.");

            foreach (Requirement r in Requirements)
                r.FindConstruction();

            nameTypeObject = null;

            if (TypeConstruction is null)
            {
                Debug.Assert(Cost > 0);
            }
            else
            {
                Debug.Assert(Cost == 0);
            }
        }
    }
}
