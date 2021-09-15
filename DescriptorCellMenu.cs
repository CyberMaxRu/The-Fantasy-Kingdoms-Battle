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
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{

    // Класс описателя ячейки меню
    internal abstract class DescriptorCellMenu : Descriptor
    {
        public DescriptorCellMenu(XmlNode n) : base()
        {
            Layer = GetIntegerNotNull(n, "Layer");
            string[] parts = GetStringNotNull(n, "Pos").Split();
            Coord = new Point(int.Parse(parts[0]) - 1, int.Parse(parts[1]) - 1);

            Cost = GetInteger(n, "Cost");
            LoadRequirements(Requirements, n);

            Debug.Assert(Coord.X >= 0);
            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y >= 0);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
        }

        internal int Layer { get; }
        internal Point Coord { get; }// Координаты ячейки
        internal int Cost { get; }// Стоимость
        internal List<Requirement> Requirements { get; } = new List<Requirement>();// Список требований

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            foreach (Requirement r in Requirements)
                r.TuneDeferredLinks();
        }
    }

    internal enum TypeCellMenuForConstruction { Research, Event, HireCreature, Build, Action };

    internal sealed class DescriptorCellMenuForConstruction : DescriptorCellMenu
    {
        public DescriptorCellMenuForConstruction(XmlNode n) : base(n)
        {
            NameEntity = GetStringNotNull(n, "Entity");

            Debug.Assert(NameEntity.Length > 0);
        }

        internal TypeCellMenuForConstruction Type { get; }
        internal string NameEntity { get; set; }
        internal DescriptorCellMenu NextCell { get; }
    }
}
