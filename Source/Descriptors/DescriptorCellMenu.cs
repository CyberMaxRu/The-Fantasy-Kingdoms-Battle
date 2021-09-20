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
            if (!int.TryParse(parts[0], out int x) || !int.TryParse(parts[1], out int y))
                throw new Exception($"Не могу распарсить координаты: {pos}.");
            Coord = new Point(x - 1, y - 1);

            Cost = GetInteger(n, "Cost");
            LoadRequirements(Requirements, n);

            Debug.Assert(Coord.X >= 0);
            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y >= 0);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
        }

        public DescriptorCellMenu(Point coord, XmlNode n) : base()
        {
            Coord = coord;
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

    internal enum TypeCellMenuForConstruction { Research, Event, HireCreature, Build, LevelUp, Action };

    internal class DescriptorCellMenuForConstruction : DescriptorCellMenu
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

        public DescriptorCellMenuForConstruction(Point coord, XmlNode n) : base(coord, n)
        {
            Type = TypeCellMenuForConstruction.LevelUp;
            Income = GetInteger(n, "Income");

            Debug.Assert(Income >= 0);
        }

        internal TypeCellMenuForConstruction Type { get; }
        internal string NameEntity { get; set; }
        internal int Income { get; }// Прибавление дохода
        internal DescriptorCellMenu NextCell { get; }
    }

    internal sealed class DescriptorCellMenuForConstructionLevel : DescriptorCellMenuForConstruction
    { 
        public DescriptorCellMenuForConstructionLevel(int number, Point coord, XmlNode n) : base(coord, n)
        {
            Number = number;
            Builders = GetIntegerNotNull(n, "Builders");
            MaxInhabitant = GetInteger(n, "MaxInhabitant");
            Capacity = GetInteger(n, "Capacity");
            GreatnessByConstruction = GetInteger(n, "GreatnessByConstruction");
            GreatnessPerDay = GetInteger(n, "GreatnessPerDay");
            BuildersPerDay = GetInteger(n, "BuildersPerDay");

            Debug.Assert(Number >= 0);
            Debug.Assert(Number <= 5);
            Debug.Assert(MaxInhabitant >= 0);
            Debug.Assert(MaxInhabitant <= 100);
            Debug.Assert(Capacity >= 0);
            Debug.Assert(Capacity <= 100);
            Debug.Assert(GreatnessByConstruction >= 0);
            Debug.Assert(GreatnessPerDay >= 0);
            Debug.Assert(BuildersPerDay >= 0);
        }

        internal int Number { get; }
        internal int Builders { get; }
        internal int MaxInhabitant { get; }
        internal int Capacity { get; }
        internal int GreatnessByConstruction { get; }// Дает очков Величия при постройке
        internal int GreatnessPerDay { get; }// Дает очков Величия в день
        internal int BuildersPerDay { get; }// Дает строителей в день
    }
}
