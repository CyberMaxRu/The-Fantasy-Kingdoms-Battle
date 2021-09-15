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
    internal sealed class DescriptorCellMenu : Descriptor
    {
        public DescriptorCellMenu(XmlNode n) : base()
        {
            Coord = new Point(XmlUtils.GetIntegerNotNull(n, "PosX") - 1, XmlUtils.GetIntegerNotNull(n, "PosY") - 1);
            Layer = XmlUtils.GetIntegerNotNull(n, "Layer") - 1;
            NameTypeObject = XmlUtils.GetStringNotNull(n, "TypeObject");
            Cost = XmlUtils.GetInteger(n, "Cost");
            XmlUtils.LoadRequirements(Requirements, n);

            Debug.Assert(Coord.X >= 0);
            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y >= 0);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
            Debug.Assert(Layer >= 0);
            Debug.Assert(Layer <= 4);
            Debug.Assert(NameTypeObject.Length > 0);
        }
    
        internal Point Coord { get; }// Координаты ячейки
        internal int Layer { get; }// Визуальный слой ячейки
        internal DescriptorEntity TypeEntity { get; set; }// Тип получаемой сущности
        internal DescriptorGroupItems GroupItems { get; }// Исследуемая группа предметов
        internal DescriptorConstruction TypeConstruction { get; set; }// Строимое сооружение
        internal List<Requirement> Requirements { get; } = new List<Requirement>();
        internal int Cost { get; }// Стоимость
        internal string NameTypeObject { get; private set; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            TypeEntity = Config.FindItem(NameTypeObject, false);

            if (TypeEntity is null)
                TypeEntity = Config.FindGroupItem(NameTypeObject, false);

            if (TypeEntity is null)
                TypeEntity = Config.FindAbility(NameTypeObject, false);

            if (TypeEntity is null)
                TypeEntity = Config.FindCreature(NameTypeObject, false);

            if (TypeEntity is null)
            {
                TypeConstruction = Config.FindConstruction(NameTypeObject, false);
            }

            if ((TypeEntity is null) && (TypeConstruction is null))
                throw new Exception("Сущность " + NameTypeObject + " не найдена.");

            foreach (Requirement r in Requirements)
                r.TuneDeferredLinks();

            if (TypeConstruction is null)
            {
                Debug.Assert(Cost > 0, $"У {NameTypeObject} не указана цена.");
            }
            else
            {
                Debug.Assert(Cost == 0, $"У {NameTypeObject} цена должна быть 0 (указана {Cost}).");
            }

            NameTypeObject = "";
        }
    }

    //internal class 
}
