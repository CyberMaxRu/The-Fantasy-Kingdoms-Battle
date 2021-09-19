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
            string pos = GetStringNotNull(n, "Pos");
            Debug.Assert(pos.Length > 0);
            string[] parts = pos.Split(',');
            int x, y;
            if (!int.TryParse(parts[0], out x) || !int.TryParse(parts[1], out y))
                throw new Exception($"Не могу распарсить координаты: {pos}.");
            Coord = new Point(x - 1, y - 1);

            Cost = GetInteger(n, "Cost");
            LoadRequirements(Requirements, n);

            Debug.Assert(Coord.X >= 0);
            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y >= 0);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
        }

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
            Type = (TypeCellMenuForConstruction)Enum.Parse(typeof(TypeCellMenuForConstruction), n.SelectSingleNode("Type").InnerText);
            NameEntity = GetStringNotNull(n, "Entity");
            Income = GetInteger(n, "Income");

            Debug.Assert(NameEntity.Length > 0);
            Debug.Assert(Income >= 0);

            XmlNode next = n.SelectSingleNode("CellMenu");
            if (next != null)
            {
                NextCell = new DescriptorCellMenuForConstruction(next);
                Debug.Assert(Coord.Equals(NextCell.Coord), $"У {NameEntity} в ячейку {Coord} вложена ячейка {NextCell.Coord}.");
            }
        }

        internal TypeCellMenuForConstruction Type { get; }
        internal string NameEntity { get; set; }
        internal int Income { get; }// Прибавление дохода
        internal DescriptorCellMenu NextCell { get; }
    }
}
